﻿namespace CarRental.Domain.Models.Rentals
{
    public class RentalDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int CustomerIdNumber { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerAddress { get; set; }
        public Guid CarId { get; set; }
        public string CarType { get; set; }
        public string CarModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
