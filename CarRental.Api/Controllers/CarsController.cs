
using CarRental.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("cars/")]
public class CarsController : ControllerBase
{

    private readonly ILogger<CarRentalController> _logger;
    private readonly ICarService _carService;


    public CarsController(
        ICarService carService,
        ILogger<CarRentalController> logger)
    {
        _carService = carService;
        _logger = logger;
    }

    [HttpGet]
    [Route("types")]
    public async Task<IActionResult> GetCarTypes()
    {
        var result = await _carService.GetCarTypes();
        return Ok(result);
    }

    [HttpGet]
    [Route("models")]
    public async Task<IActionResult> GetCarModels()
    {
        var result = await _carService.GetCarModels();
        return Ok(result);
    }


}
