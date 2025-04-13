using CarRental.DB.Repositories;
using CarRental.Domain.Mappers;
using CarRental.DB.Entities;
using CarRental.Domain.Models.Customers;
using Microsoft.Extensions.Configuration;

namespace CarRental.Domain.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICarRentalDbUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly int _customerCacheDurationInHours;
        public CustomerService(ICarRentalDbUnitOfWork unitOfWork, ICacheService cacheService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _customerCacheDurationInHours = int.Parse(configuration.GetSection("Caching:CustomerCacheDurationInHours").Value);
        }

        /// <summary>
        /// Registers a customer in the system
        /// </summary>
        /// <param name="personalIdNumber">Id number (e.g DNI) of the customer</param>
        /// <param name="fullName">The full name of the customer</param>
        /// <param name="address">The address of the customer</param>
        /// <returns></returns>
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

            var customerDto = CustomerMapper.MapToDto(customer);
            _cacheService.Set(GetCustomerKey(personalIdNumber), customerDto, TimeSpan.FromHours(_customerCacheDurationInHours));

            return customerDto;
        }

        /// <summary>
        /// Gets a customer by their personal id number (e.g DNI)
        /// </summary>
        /// <param name="personalIdNumber">The personal id number of the customer</param>
        /// <returns></returns>
        public async Task<CustomerDto> GetCustomerAsync(int personalIdNumber)
        {
            return await _cacheService.GetOrSetAsync(GetCustomerKey(personalIdNumber), async () =>
            {
                var customer = (await _unitOfWork.Customers.FindAsync(c => c.PersonalIdNumber == personalIdNumber)).FirstOrDefault();
                return CustomerMapper.MapToDto(customer);
            }, TimeSpan.FromHours(_customerCacheDurationInHours));

        }

        private string GetCustomerKey(int id) { 
            return $"customer-{id}";
        }
    }
}
