using CarRental.Domain.Models.Rentals;

namespace CarRental.Domain.Models.Cars
{
    public class CarDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }

        public IEnumerable<ServiceDto> Services { get; set; }
        public IEnumerable<RentalDto> Rentals { get; set; }

        public bool IsUnavailableForService(DateTime startDate, DateTime endDate)
        {
            return Services != null && Services.Any(s =>
                startDate <= s.StartDate.AddDays(s.DurationInDays - 1) &&
                endDate >= s.StartDate);
        }
    }
}
