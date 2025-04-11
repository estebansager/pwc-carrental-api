using CarRental.Domain.Models.Cars;
using CarRental.Domain.Models.Customers;
using CarRental.Domain.Models.Rentals;

namespace CarRental.Domain.Services
{
    public interface ICarRentalService
    {
        Task<IEnumerable<RentalDto>> GetRentalsAsync();
        Task<IEnumerable<CarDto>> CheckAvailabilityAsync(DateTime startDate, DateTime endDate, CarType? type = null, string model = null);
        Task<bool> IsAvailable(Guid carId, DateTime startDate, DateTime endDate, Guid? rentalId);
        Task<RentalDto> RegisterRentalAsync(Guid customerId, Guid carId, DateTime startDate, DateTime endDate);
        Task<RentalDto> ModifyReservationAsync(Guid rentalId, DateTime? newStartDate = null, DateTime? newEndDate = null, Guid? newCarId = null);
        Task CancelRentalAsync(Guid rentalId);

    }
}
