using CarRental.Domain.Models.Cars;
using CarRental.Domain.Models.Rentals;

namespace CarRental.Domain.Services
{
    public interface ICarRentalService
    {
        Task<IEnumerable<RentalDto>> GetRentalsAsync();
        Task<RentalDto> GetRentalByIdAsync(Guid rentalId);
        Task<IEnumerable<CarAvailabilityDto>> CheckAvailabilityAsync(DateTime startDate, DateTime endDate, CarType? type = null, string model = null);
        Task<RentalDto> RegisterRentalAsync(Guid customerId, string carModel, CarType carType, DateTime startDate, DateTime endDate);
        Task<RentalDto> ModifyReservationAsync(Guid rentalId, DateTime? newStartDate = null, DateTime? newEndDate = null, string? newCarModel = null, CarType? newCarType = null);
        Task CancelRentalAsync(Guid rentalId);

    }
}
