using CarRental.DB.Entities;
using System.Data;

namespace CarRental.DB.Repositories
{
    public interface ICarRentalDbUnitOfWork
    {
        ICarRentalDbRepository<Car> Cars { get; set; }
        ICarRentalDbRepository<Service> Services { get; set; }
        ICarRentalDbRepository<Customer> Customers { get; set; }
        ICarRentalDbRepository<Rental> Rentals { get; set; }



        Task<int> SaveChangesAsync();
        Task ExecuteTransactionAsync(Func<Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

    }
}
