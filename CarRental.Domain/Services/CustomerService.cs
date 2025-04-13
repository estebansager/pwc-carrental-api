using CarRental.DB.Repositories;
using CarRental.Domain.Mappers;
using CarRental.DB.Entities;
using CarRental.Domain.Models.Customers;

namespace CarRental.Domain.Services
{
    public class CustomerService : ICustomerService
    {
        private ICarRentalDbUnitOfWork _unitOfWork { get; }
        public CustomerService(ICarRentalDbUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerDto> RegisterCustomerAsync(int personalIdNumber, string fullName, string address)
        {
            var customer = new Customer()
            {
                Id = Guid.NewGuid(),
                PersonalIdNumber = personalIdNumber,
                Address = address,
                FullName = fullName,
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return CustomerMapper.MapToDto(customer);
        }

        public async Task<CustomerDto> GetCustomerAsync(int personalIdNumber)
        {
            var customer = (await _unitOfWork.Customers.FindAsync(c => c.PersonalIdNumber == personalIdNumber)).FirstOrDefault();
            return CustomerMapper.MapToDto(customer);
        }
    }
}
