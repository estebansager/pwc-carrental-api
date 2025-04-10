using CarRental.Domain.Models.Cars;

namespace CarRental.Domain.Models.Customers
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }

        public IEnumerable<Rental> Rentals { get; set; }
    }
}
