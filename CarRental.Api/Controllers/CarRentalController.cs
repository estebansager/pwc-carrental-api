
using CarRental.Domain.Exceptions;
using CarRental.Domain.Models.Cars;
using CarRental.Domain.Models.Customers;
using CarRental.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("rentals/")]
public class CarRentalController : ControllerBase
{

    private readonly ILogger<CarRentalController> _logger;
    private readonly ICarRentalService _carRentalService;
    private readonly ICustomerService _customerService;


    public CarRentalController(
        ICarRentalService carRentalService,
        ICustomerService customerService,
        ILogger<CarRentalController> logger)
    {
        _carRentalService = carRentalService;
        _customerService = customerService;
        _logger = logger;
    }

    [HttpGet]
    [Route("availability")]
    public async Task<IActionResult> CheckAvailability(DateTime startDate, DateTime endDate, string? model, CarType? type)
    {
        var result = await _carRentalService.CheckAvailabilityAsync(startDate, endDate, type, model);
        return Ok(result);
    }

    [HttpGet]
    [Route("availability/{carId}")]
    public async Task<IActionResult> CheckAvailability([FromRoute]Guid carId, DateTime startDate, DateTime endDate)
    {
        var result = await _carRentalService.IsAvailable(carId, startDate, endDate, null);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterRental([FromBody]RentalIn rentalIn)
    {

        try
        {
            /* First check if the customer exists or needs to be created */
            CustomerDto customer = await _customerService.GetCustomerAsync(rentalIn.CustomerIdNumber);
            if (customer == null) {
                customer = await _customerService.RegisterCustomerAsync(rentalIn.CustomerIdNumber, rentalIn.CustomerFullName, rentalIn.CustomerAddress);
            }

            var result = await _carRentalService.RegisterRentalAsync(customer.Id, rentalIn.CarId, rentalIn.StartDate, rentalIn.EndDate);
            return Ok(result);
        }
        catch (CarNotAvailableException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidRentDatesException ird)
        {
            return BadRequest(ird.Message);
        }
    }


    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateRental(Guid id, [FromBody] RentalUpdateIn rentalUpdateIn)
    {

        try
        {
            var result = await _carRentalService.ModifyReservationAsync(id, rentalUpdateIn.StartDate, rentalUpdateIn.EndDate, rentalUpdateIn.CarId);
            return Ok(result);
        }
        catch (CarNotAvailableException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidRentDatesException ird)
        {
            return BadRequest(ird.Message);
        }
        catch (RentalNotFoundException rnf)
        {
            return NotFound(rnf.Message);
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetRentals()
    {
        var result = await _carRentalService.GetRentalsAsync();
        return Ok(result);

    }


    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> CancelRental(Guid id)
    {
        try
        {
            await _carRentalService.CancelRentalAsync(id);
            return Ok("Rental deleted successfully");

        }
        catch (RentalNotFoundException rnf)
        {
            return NotFound(rnf.Message);
        }
        catch (RentalCancellationException rce)
        {
            return BadRequest(rce.Message);
        }

    }
}
