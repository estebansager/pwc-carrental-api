namespace CarRental.DB.Entities
{
    public class Car
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }

        public IEnumerable<Service> Services { get; set; }
        public IEnumerable<Rental> Rentals { get; set; }
     
    }
}
