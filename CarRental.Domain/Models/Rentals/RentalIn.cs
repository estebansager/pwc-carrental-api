namespace CarRental.Domain.Models.Cars
{
    public class RentalIn
    {
        public int CustomerIdNumber { get; set; }
        public string CustomerFullName{ get; set; }
        public string CustomerAddress { get; set; }

        public CarType CarType { get; set; }
        public string CarModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
