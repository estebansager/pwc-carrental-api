using CarRental.DB.Entities;
using CarRental.Domain.Models.Customers;

namespace CarRental.Domain.Mappers
{
    internal class CustomerMapper
    {

            public static CustomerDto MapToDto(Customer customer)
            {
            if (customer == null)
                return null;

                return new CustomerDto()
                {
                    Id = customer.Id,
                    PersonalIdNumber = customer.PersonalIdNumber,
                    Address = customer.Address,
                    FullName = customer.FullName
                };

            }
    }
}
