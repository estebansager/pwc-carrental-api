
using CarRental.Domain.Exceptions;
using CarRental.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("reporting/")]
public class RentalReportingController : ControllerBase
{

    private readonly IRentalReportingService _rentalReportingService;
    private readonly ILogger<RentalReportingController> _logger;


    public RentalReportingController(
        IRentalReportingService rentalReportingService,
        ILogger<RentalReportingController> logger)
    {
        _rentalReportingService = rentalReportingService;
        _logger = logger;
    }

    [HttpGet]
    [Route("mostrentedcar")]
    public async Task<IActionResult> GetMostUsedCarType(DateTime startDate, DateTime endDate)
    {
        try
        {
            var result = await _rentalReportingService.GetMostRentedCarTypeAsync(startDate, endDate);
            return Ok(result);
        }
        catch (InvalidRentDatesException id)
        {
            return BadRequest(id.Message);
        }
    }

    [HttpGet]
    [Route("scheduledservices")]
    public async Task<IActionResult> GetNextServices()
    {
        var result = await _rentalReportingService.GetScheduledServicesNextTwoWeeksAsync();
        return Ok(result);
    }

}
