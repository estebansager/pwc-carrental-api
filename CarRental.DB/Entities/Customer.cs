namespace CarRental.DB.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public int PersonalIdNumber { get; set; } 
        public string FullName { get; set; }
        public string Address { get; set; }

        public IEnumerable<Rental> Rentals { get; set; }
    }
}
