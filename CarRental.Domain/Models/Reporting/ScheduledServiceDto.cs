public class ScheduledServiceDto
{
    public Guid CarId { get; set; }
    public string CarType { get; set; }
    public string Model { get; set; }
    public DateTime ServiceStartDate { get; set; }
    public int DurationInDays { get; set; }
}