using CarRental.DB.Contexts;
using CarRental.DB.Entities;

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

    }
}
