using CarRental.DB.Contexts;
using CarRental.DB.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CarRental.DB.Repositories
{
    public class CarRentalDbUnitOfWork : ICarRentalDbUnitOfWork
    {
        private readonly CarRentalDbContext _context;

        public ICarRentalDbRepository<Car> Cars { get; set; }
        public ICarRentalDbRepository<Service> Services { get; set; }
        public ICarRentalDbRepository<Customer> Customers { get; set; }
        public ICarRentalDbRepository<Rental> Rentals { get; set; }


        public CarRentalDbUnitOfWork(CarRentalDbContext context)
        {
            this.Cars = new CarRentalDbRepository<Car>(context);
            this.Services = new CarRentalDbRepository<Service>(context);
            this.Customers = new CarRentalDbRepository<Customer>(context);
            this.Rentals = new CarRentalDbRepository<Rental>(context);

            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Executes a Database action as a transaction with an isolation level. This is important for race conditions over rentals, for example
        /// </summary>
        /// <param name="action">The action to run (against the DB)</param>
        /// <param name="isolationLevel">The DB isolatioDFn level to use in the transaction</param>
        /// <returns></returns>
        public async Task ExecuteTransactionAsync(Func<Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel);
            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
