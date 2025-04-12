using CarRental.Domain.Models.Cars;
using CarRental.Domain.Models.Rentals;
using Microsoft.Extensions.Primitives;

namespace CarRental.Domain.Services
{
    public interface ICarRentalService
    {
        Task<IEnumerable<RentalDto>> GetRentalsAsync();
        Task<IEnumerable<CarAvailabilityDto>> CheckAvailabilityAsync(DateTime startDate, DateTime endDate, CarType? type = null, string model = null);
        Task<bool> IsAvailable(Guid carId, DateTime startDate, DateTime endDate, Guid? rentalId);
        Task<CarDto?> GetAvailableCar(CarType carType, string carModel, DateTime startDate, DateTime endDate, Guid? rentalId = null);
        Task<RentalDto> RegisterRentalAsync(Guid customerId, string carModel, CarType carType, DateTime startDate, DateTime endDate);
        Task<RentalDto> ModifyReservationAsync(Guid rentalId, DateTime? newStartDate = null, DateTime? newEndDate = null, Guid? newCarId = null);
        Task CancelRentalAsync(Guid rentalId);

    }
}
