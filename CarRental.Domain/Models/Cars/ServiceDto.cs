namespace CarRental.Domain.Models.Cars
{
    public class ServiceDto
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; } 

        public Guid CarId { get; set; }
        public CarDto Car { get; set; }

        public int DurationInDays { get; set; } = 2; // Always 2 days
        public bool IsCompleted { get; set; }
    }
}
