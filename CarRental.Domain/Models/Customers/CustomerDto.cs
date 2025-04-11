using CarRental.Domain.Models.Rentals;

namespace CarRental.Domain.Models.Customers
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public int PersonalIdNumber { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }

    }
}
