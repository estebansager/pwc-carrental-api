namespace CarRental.Domain.Models.Cars
{
    public class RentalUpdateIn
    {
        public Guid? CarId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
