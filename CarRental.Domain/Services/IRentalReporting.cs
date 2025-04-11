namespace CarRental.Domain.Services
{
    public interface IRentalReportingService
    {
        Task<IEnumerable<ScheduledServiceDto>> GetScheduledServicesNextTwoWeeksAsync();
        Task<MostRentedCarTypeDto> GetMostRentedCarTypeAsync(DateTime startDate, DateTime endDate);

    }
}
