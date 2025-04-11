using CarRental.Domain.Models.Cars;
using CarRental.Domain.Models.Customers;

namespace CarRental.Domain.Services
{
    public interface ICarService
    {
        Task<IEnumerable<(CarDto Car, DateTime ServiceDate)>> GetScheduledServicesAsync(DateTime fromDate, DateTime toDate);

    }
}
