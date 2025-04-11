    
using CarRental.DB.Repositories;
using CarRental.Domain.Exceptions;

namespace CarRental.Domain.Services
{
    public class RentalReportingService : IRentalReportingService
    {
        private ICarRentalDbUnitOfWork _unitOfWork { get; }
        public RentalReportingService(ICarRentalDbUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MostRentedCarTypeDto> GetMostRentedCarTypeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new InvalidRentDatesException("the end date must be greater than the start date");

            var rentals = await _unitOfWork.Rentals.FindAsync(
                r => r.StartDate.Date <= endDate.Date && r.EndDate.Date >= startDate.Date,
                r => r.Car
            );

            if (!rentals.Any())
                return new MostRentedCarTypeDto { CarType = "None", UtilizationPercentage = 0 }; //default state

            var rentalsByType = rentals
                .Where(r => r.Car != null)
                .GroupBy(r => r.Car.Type)
                .Select(g => new { CarType = g.Key, Count = g.Count() })
                .ToList();

            var totalCount = rentalsByType.Sum(g => g.Count);
            var mostRented = rentalsByType.OrderByDescending(g => g.Count).First();

            var percentage = (double)mostRented.Count / totalCount * 100;

            return new MostRentedCarTypeDto
            {
                CarType = mostRented.CarType,
                UtilizationPercentage = Math.Round(percentage, 2)
            };
        }


        public async Task<IEnumerable<ScheduledServiceDto>> GetScheduledServicesNextTwoWeeksAsync()
        {
            var today = DateTime.UtcNow.Date;
            var inTwoWeeks = today.AddDays(14);

            var cars = await _unitOfWork.Cars.FindAsync(
                c => c.Services.Any(s => s.StartDate.Date >= today && s.StartDate.Date <= inTwoWeeks),
                x => x.Services
            );

            var scheduledServices = cars
                .SelectMany(car => car.Services
                    .Where(s => s.StartDate >= today && s.StartDate <= inTwoWeeks)
                    .Select(s => new ScheduledServiceDto
                    {
                        CarId = car.Id,
                        CarType = car.Type,
                        Model = car.Model,
                        ServiceStartDate = s.StartDate,
                        DurationInDays = s.DurationInDays
                    }));

            return scheduledServices.OrderBy(s => s.ServiceStartDate).ToList();
        }
    }
}
