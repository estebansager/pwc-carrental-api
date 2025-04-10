using CarRental.Domain.Models.Cars;
using CarRental.Domain.Models.Customers;

namespace CarRental.DB.Repositories
{
    public interface ICarRentalDbUnitOfWork
    {
        ICarRentalDbRepository<Car> Cars { get; set; }
        ICarRentalDbRepository<Service> Services { get; set; }
        ICarRentalDbRepository<Customer> Customers { get; set; }
        ICarRentalDbRepository<Rental> Rentals { get; set; }



        Task<int> SaveChangesAsync();

    }
}
