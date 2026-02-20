using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/users/me/cars")]
[Authorize]
public class UserCarsController : ControllerBase
{
    private readonly ICarService _carService;

    public UserCarsController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserCarDto>>> GetUserCars(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var cars = await _carService.GetUserCarsAsync(userId, cancellationToken);
        return Ok(cars);
    }

    [HttpGet("{carId}")]
    public async Task<ActionResult<UserCarDto>> GetUserCar(int carId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var car = await _carService.GetUserCarByIdAsync(userId, carId, cancellationToken);

        if (car == null)
            return NotFound();

        return Ok(car);
    }

    [HttpPost]
    public async Task<ActionResult<UserCarDto>> CreateUserCar([FromBody] CreateUserCarRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            var subTier = User.GetSubscriptionTier();
            var car = await _carService.CreateUserCarAsync(subTier, userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetUserCar), new { carId = car.Id }, car);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{carId}")]
    public async Task<ActionResult<UserCarDto>> UpdateUserCar(int carId, [FromBody] UpdateUserCarRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var car = await _carService.UpdateUserCarAsync(userId, carId, request, cancellationToken);

        if (car == null)
            return NotFound();

        return Ok(car);
    }

    [HttpDelete("{carId}")]
    public async Task<IActionResult> DeleteUserCar(int carId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.GetUserId();
            var deleted = await _carService.DeleteUserCarAsync(userId, carId, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("can-add")]
    public async Task<ActionResult<CarLimitDto>> CanAddCar(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var subscriptionTier = User.GetSubscriptionTier();
        return await _carService.CanUserAddCarAsync(subscriptionTier, userId, cancellationToken);
    }
}
