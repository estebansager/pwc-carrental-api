namespace CarRental.Domain.Models.Cars
{
    public class RentalUpdateIn
    {
        public string CarModel { get; set; }
        public CarType CarType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
