
using CarRental.Domain.Models.Cars;
using CarRental.DB.Repositories;
using CarRental.Domain.Mappers;
using CarRental.DB.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models.Rentals;
using Microsoft.Extensions.Logging;


namespace CarRental.Domain.Services
{
    public class CarRentalService : ICarRentalService
    {
        private ICarRentalDbUnitOfWork _unitOfWork;
        private ILogger<CarRentalService> _logger;
        public CarRentalService(ICarRentalDbUnitOfWork unitOfWork, ILogger<CarRentalService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }




        public async Task CancelRentalAsync(Guid rentalId)
        {
            var rental = await GetRentalAsync(rentalId);

            /* As an assumption, an existing rental can be cancelled only if the start date is in the future */
            if (rental.StartDate.Date < DateTime.UtcNow.AddDays(1).Date)
                throw new RentalCancellationException("The provided rental cannot be cancelled. Make sure it has a starting rental date in the future");


            /* For simplicity I'm doing psysical deletes, but it could perfectly be a logical delete for auditing and reporting purposes */
            _unitOfWork.Rentals.Remove(rental);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<RentalDto>> GetRentalsAsync()
        {
            var rentals = await _unitOfWork.Rentals.FindAsync(r => true);
            return rentals.Select(RentalMapper.MapToDto);
        }


        public async Task<IEnumerable<CarAvailabilityDto>> CheckAvailabilityAsync(DateTime startDate, DateTime endDate, CarType? type = null, string model = null)
        {

            if (endDate < startDate || startDate < DateTime.UtcNow.Date)
                throw new InvalidRentDatesException("Invalid dates provided. Make sure you are checking availability from a present or future date and that the end date is greater than the start date");

            var result = await _unitOfWork.Cars.FindAsync(
                c =>
                    (!type.HasValue || c.Type.ToLower() == type.Value.ToString().ToLower()) &&
                    (string.IsNullOrEmpty(model) || c.Model.ToLower().Contains(model.ToLower())) &&
                    !c.Rentals.Any(r => startDate <= r.EndDate.AddDays(1) && endDate >= r.StartDate) &&
                    !c.Services.Any(s => startDate <= s.StartDate.AddDays(s.DurationInDays - 1) && endDate >= s.StartDate),
                x => x.Rentals,
                x => x.Services
            );

            return result.GroupBy(r => (r.Type, r.Model)).Select(g => 
                new CarAvailabilityDto() {
                    Model = g.Key.Model,
                    Type = g.Key.Type,
                    Count = g.Count()
                });
        }

        public async Task<bool> IsAvailable(Guid carId, DateTime startDate, DateTime endDate, Guid? rentalId = null)
        {
            var car = (await _unitOfWork.Cars.FindAsync(
                c =>
                    c.Id == carId &&
                    !c.Rentals.Any(r => 
                            r.Id != rentalId /*If checking availability for dates of the same rental, do not consider it*/
                            && startDate <= r.EndDate.AddDays(1) 
                            && endDate >= r.StartDate) &&
                    !c.Services.Any(s => startDate <= s.StartDate.AddDays(s.DurationInDays - 1) && endDate >= s.StartDate),
                x => x.Rentals,
                x => x.Services
            )).FirstOrDefault();

            return car != null;
        }

        public async Task<CarDto?> GetAvailableCar(CarType carType, string carModel, DateTime startDate, DateTime endDate, Guid? rentalId = null)
        {
            var car = (await _unitOfWork.Cars.FindAsync(
                c =>
                    c.Type == carType.ToString() &&
                    c.Model == carModel &&
                    !c.Rentals.Any(r =>
                            r.Id != rentalId /*If checking availability for dates of the same rental, do not consider it*/
                            && startDate <= r.EndDate.AddDays(1)
                            && endDate >= r.StartDate) &&
                    !c.Services.Any(s => startDate <= s.StartDate.AddDays(s.DurationInDays - 1) && endDate >= s.StartDate),
                x => x.Rentals,
                x => x.Services
            )).FirstOrDefault();

            if (car == null)
                return null;

            return CarMapper.MapToDto(car);
        }


        public async Task<RentalDto> ModifyReservationAsync(Guid rentalId, DateTime? newStartDate = null, DateTime? newEndDate = null, Guid? newCarId = null)
        {
            var rental = await GetRentalAsync(rentalId);

            var selectedCarId = newCarId.HasValue ? newCarId.Value : rental.CarId;
            var selectedStartDate = newStartDate.HasValue ? newStartDate.Value : rental.StartDate;
            var selectedEndDate = newEndDate.HasValue ? newEndDate.Value : rental.EndDate;

            if (selectedStartDate.Date < DateTime.UtcNow.Date || selectedEndDate.Date < selectedStartDate.Date)
                throw new InvalidRentDatesException("The provided date range is invalid. Make sure the starting date is not in the past and that the ending date is greater than the starting date");


            if (selectedCarId == rental.CarId && selectedStartDate == rental.StartDate && selectedEndDate == rental.EndDate) {
                /* nothing to update, return */
                return RentalMapper.MapToDto(rental);
            }

            /* if at least one attribute changed, check if there is availability before updating the rental */
            var isAvailable = await IsAvailable(selectedCarId, selectedStartDate, selectedEndDate, rental.Id);
            if (!isAvailable)
            {
                _logger.LogWarning($"Customer {rental.CustomerId} attempted to update a rental without availability for car {selectedCarId} from {selectedStartDate} to {selectedEndDate}");
                throw new CarNotAvailableException("The selected car with the provided dates is not available for renting");
            }

            rental.StartDate = selectedStartDate;
            rental.EndDate = selectedEndDate;
            rental.CarId = selectedCarId;

            _unitOfWork.Rentals.Update(rental);
            await _unitOfWork.SaveChangesAsync();

            return RentalMapper.MapToDto(rental);
        }

        /// <summary>
        /// Registers a rental for a customer.
        /// </summary>
        /// <param name="customerPersonalIdNumber">The personal identification number (e.g DNI) that identifies the customer</param>
        /// <param name="carId">The car to rent</param>
        /// <param name="startDate">Starting date of the rental</param>
        /// <param name="endDate">Ending date of the rental</param>
        /// <returns></returns>
        public async Task<RentalDto> RegisterRentalAsync(Guid customerId, string carModel, CarType carType, DateTime startDate, DateTime endDate)
        {

            /* First validate that the startDate is not in the past and that endDate is greater than startDate */
            if (startDate.Date < DateTime.UtcNow.Date || endDate.Date < startDate.Date)
                throw new InvalidRentDatesException("The provided date range is invalid. Make sure the starting date is not in the past and that the ending date is greater than the starting date");

            /* Now validate that the car is available for renting */
            var car = await GetAvailableCar(carType, carModel, startDate, endDate);
            if (car == null) {
                /* User tried to rent a car that is now available. Might be important to log for auditing purposes */
                _logger.LogWarning($"Customer {customerId} attempted to rent unavailable car model {carModel} and car type {carType} from {startDate} to {endDate}");
                throw new CarNotAvailableException("The selected car model and type is not available for renting for the provided dates");
            }


            var rental = new Rental
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                CarId = car.Id,
                StartDate = startDate,
                EndDate = endDate
            };

            await _unitOfWork.Rentals.AddAsync(rental);
            await _unitOfWork.SaveChangesAsync();
            return RentalMapper.MapToDto(rental);
        }



        private async Task<Rental> GetRentalAsync(Guid rentalId) {
            var rental = (await _unitOfWork.Rentals.FindAsync(r => r.Id == rentalId)).FirstOrDefault();
            if (rental == null)
                throw new RentalNotFoundException("The provided rental could not be found");

            return rental;
        }
    }
}

