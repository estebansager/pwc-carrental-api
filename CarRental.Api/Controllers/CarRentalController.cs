
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("rentals")]
public class CarRentalController : ControllerBase
{

    private readonly ILogger<CarRentalController> _logger;

    public CarRentalController(ILogger<CarRentalController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetRentals()
    {
        return await Task.FromResult(Ok("success"));
    }
}
