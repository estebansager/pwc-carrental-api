namespace CarRental.DB.Entities
{
    public class Service
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; } 

        public Guid CarId { get; set; }
        public Car Car { get; set; }

        public int DurationInDays { get; set; } = 2;
        public bool IsCompleted { get; set; }
    }
}
