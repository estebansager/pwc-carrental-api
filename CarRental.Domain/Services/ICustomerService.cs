using CarRental.Domain.Models.Customers;

namespace CarRental.Domain.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto> RegisterCustomerAsync(int personalIdNumber, string fullName, string address);
        Task<CustomerDto> GetCustomerAsync(int personalIdNumber);


    }
}
