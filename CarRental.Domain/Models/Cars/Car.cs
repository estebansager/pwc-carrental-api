namespace CarRental.Domain.Models.Cars
{
    public class Car
    {
        public Guid Id { get; set; }
        public CarType Type { get; set; }
        public string Model { get; set; }

        public IEnumerable<Service> Services { get; set; }
        public IEnumerable<Rental> Rentals { get; set; }

        public bool IsUnavailableForService(DateTime startDate, DateTime endDate)
        {
            return Services != null && Services.Any(s =>
                startDate <= s.StartDate.AddDays(s.DurationInDays - 1) &&
                endDate >= s.StartDate);
        }
    }
}
