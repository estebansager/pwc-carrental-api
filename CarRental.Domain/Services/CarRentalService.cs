using CarRental.Domain.Models.Cars;
using CarRental.DB.Repositories;
using CarRental.Domain.Mappers;
using CarRental.DB.Entities;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models.Rentals;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarRental.Domain.Services
{
    public class CarRentalService : ICarRentalService
    {
        private ICarRentalDbUnitOfWork _unitOfWork;
        private ILogger<CarRentalService> _logger;
        private readonly RentalSettings _settings;
        public CarRentalService(ICarRentalDbUnitOfWork unitOfWork, ILogger<CarRentalService> logger, IOptions<RentalSettings> settings)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _settings = settings.Value;
        }



        /// <summary>
        /// Cancels a rental by id
        /// </summary>
        /// <param name="rentalId"> The id of the rental to cancel</param>
        /// <returns></returns>
        /// <exception cref="RentalCancellationException"></exception>
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

        /// <summary>
        /// Gets all existing rentals
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<RentalDto>> GetRentalsAsync()
        {
            var rentals = await _unitOfWork.Rentals.FindAsync(r => true, x => x.Customer, x => x.Car);
            return rentals.Select(RentalMapper.MapToDto);
        }

        /// <summary>
        /// Gets a rental by id
        /// </summary>
        /// <param name="rentalId">The id of the rental to retrieve</param>
        /// <returns></returns>
        public async Task<RentalDto> GetRentalByIdAsync(Guid rentalId)
        {
            return RentalMapper.MapToDto(await GetRentalAsync(rentalId));
        }


        /// <summary>
        /// Checks the availability of cars grouped by type and model
        /// </summary>
        /// <param name="startDate">starting date for the search</param>
        /// <param name="endDate">ending date for the search</param>
        /// <param name="type">type of the car</param>
        /// <param name="model">model of the car</param>
        /// <returns></returns>
        /// <exception cref="InvalidRentDatesException"></exception>
        public async Task<IEnumerable<CarAvailabilityDto>> CheckAvailabilityAsync(DateTime startDate, DateTime endDate, CarType? type = null, string model = null)
        {

            if (endDate < startDate || startDate < DateTime.UtcNow.Date)
                throw new InvalidRentDatesException("Invalid dates provided. Make sure you are checking availability from a present or future date and that the end date is greater than the start date");

            var result = await _unitOfWork.Cars.FindAsync(
                c =>
                    (!type.HasValue || c.Type.ToLower() == type.Value.ToString().ToLower()) &&
                    (string.IsNullOrEmpty(model) || c.Model.ToLower().Contains(model.ToLower())) &&
                    !c.Rentals.Any(r => startDate <= r.EndDate.AddDays(_settings.DaysAfterRentalEndsToMakeCarAvailable) && endDate >= r.StartDate) &&
                    !c.Services.Any(s => startDate <= s.StartDate.AddDays(s.DurationInDays) && endDate >= s.StartDate),
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

        /// <summary>
        /// Updates a rental by id.
        /// </summary>
        /// <param name="rentalId">The id of the rental to modify</param>
        /// <param name="newStartDate">new starting date of the rental</param>
        /// <param name="newEndDate">new ending date of the rental</param>
        /// <param name="newCarModel">new car model to rent</param>
        /// <param name="newCarType">new car type to rent</param>
        /// <returns></returns>
        /// <exception cref="InvalidRentDatesException"></exception>
        /// <exception cref="CarNotAvailableException"></exception>
        public async Task<RentalDto> ModifyReservationAsync(Guid rentalId, DateTime? newStartDate = null, DateTime? newEndDate = null, string? newCarModel = null, CarType? newCarType = null)
        {
            var rental = await GetRentalAsync(rentalId);

            var selectedStartDate = newStartDate.HasValue ? newStartDate.Value : rental.StartDate;
            var selectedEndDate = newEndDate.HasValue ? newEndDate.Value : rental.EndDate;

            if (selectedStartDate.Date < DateTime.UtcNow.Date || selectedEndDate.Date < selectedStartDate.Date)
                throw new InvalidRentDatesException("The provided date range is invalid. Make sure the starting date is not in the past and that the ending date is greater than the starting date");


            if (newCarModel == rental.Car.Model && newCarType?.ToString() == rental.Car.Type && selectedStartDate == rental.StartDate && selectedEndDate == rental.EndDate) {
                /* nothing to update, return */
                return RentalMapper.MapToDto(rental);
            }

            /* if at least one attribute changed, check if there is availability before updating the rental */
            var newCar = await GetAvailableCar(newCarType.Value, newCarModel, selectedStartDate, selectedEndDate, rental.Id);
            if (newCar == null)
            {
                _logger.LogWarning($"Customer {rental.CustomerId} attempted to update a rental without availability for car model {newCarModel} and car type {newCarType} from {selectedStartDate} to {selectedEndDate}");
                throw new CarNotAvailableException("The selected car type and model with the provided dates is not available for renting");
            }

            rental.StartDate = selectedStartDate;
            rental.EndDate = selectedEndDate;
            rental.CarId = newCar.Id;

            _unitOfWork.Rentals.Update(rental);
            await _unitOfWork.SaveChangesAsync();

            return RentalMapper.MapToDto(rental);
        }

        /// <summary>
        /// Registers a rental for a customer.
        /// </summary>
        /// <param name="carType">The car type to rent</param>
        /// <param name="carModel">The car model to rent</param>
        /// <param name="customerId">The id of the customer</param>
        /// <param name="startDate">Starting date of the rental</param>
        /// <param name="endDate">Ending date of the rental</param>
        /// <returns></returns>
        public async Task<RentalDto> RegisterRentalAsync(Guid customerId, string carModel, CarType carType, DateTime startDate, DateTime endDate)
        {

            /* First validate that the startDate is not in the past and that endDate is greater than startDate */
            if (startDate.Date < DateTime.UtcNow.Date || endDate.Date < startDate.Date)
                throw new InvalidRentDatesException("The provided date range is invalid. Make sure the starting date is not in the past and that the ending date is greater than the starting date");

            RentalDto result = default;

            await _unitOfWork.ExecuteTransactionAsync(async () =>
            {

                /* Now validate that the car is available for renting */
                var car = await GetAvailableCar(carType, carModel, startDate, endDate);
                if (car == null)
                {
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

                /* Load the rental to populate navigation properties */
                rental = (await _unitOfWork.Rentals.FindAsync(r => r.Id == rental.Id,
                        r => r.Customer,
                        r => r.Car)).FirstOrDefault();

                result = RentalMapper.MapToDto(rental);



            }, System.Data.IsolationLevel.Serializable); //Lock the table to avoid race conditions generating bad rentals


            return result;
        }



        private async Task<CarDto?> GetAvailableCar(CarType? carType, string carModel, DateTime startDate, DateTime endDate, Guid? rentalId = null)
        {
            var car = (await _unitOfWork.Cars.FindAsync(
                c =>
                    c.Type == (carType.HasValue ? carType.Value.ToString() : c.Type) &&
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



        private async Task<Rental> GetRentalAsync(Guid rentalId) {
            var rental = (await _unitOfWork.Rentals.FindAsync(r => r.Id == rentalId, x => x.Car, x => x.Customer)).FirstOrDefault();
            if (rental == null)
                throw new RentalNotFoundException("The provided rental could not be found");

            return rental;
        }
    }
}

