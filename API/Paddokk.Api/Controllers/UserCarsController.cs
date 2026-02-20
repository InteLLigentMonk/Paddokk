using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs;
using Paddokk.Core.Models.Entities;

namespace API.Controllers;

[ApiController]
[Route("api/users/me/cars")]
[Authorize]
public class UserCarsController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly ILogger<UserCarsController> _logger;

    public UserCarsController(ICarService carService, ILogger<UserCarsController> logger)
    {
        _carService = carService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserCarDto>>> GetUserCars()
    {
        var userId = User.GetUserId();
        var cars = await _carService.GetUserCarsAsync(userId);
        return Ok(cars);
    }

    [HttpGet("{carId}")]
    public async Task<ActionResult<UserCarDto>> GetUserCar(int carId)
    {
        var userId = User.GetUserId();
        var car = await _carService.GetUserCarByIdAsync(userId, carId);

        if (car == null)
            return NotFound();

        return Ok(car);
    }

    [HttpPost]
    public async Task<ActionResult<UserCarDto>> CreateUserCar([FromBody] CreateUserCarRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var subTier = User.GetSubscriptionTier();
            var car = await _carService.CreateUserCarAsync(subTier, userId, request);
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
    public async Task<ActionResult<UserCarDto>> UpdateUserCar(int carId, [FromBody] UpdateUserCarRequest request)
    {
        var userId = User.GetUserId();
        var car = await _carService.UpdateUserCarAsync(userId, carId, request);

        if (car == null)
            return NotFound();

        return Ok(car);
    }

    [HttpDelete("{carId}")]
    public async Task<IActionResult> DeleteUserCar(int carId)
    {
        try
        {
            var userId = User.GetUserId();
            var deleted = await _carService.DeleteUserCarAsync(userId, carId);

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
    public async Task<ActionResult<object>> CanAddCar()
    {
        var userId = User.GetUserId();
        var subTier = User.GetSubscriptionTier();
        var canAdd = await _carService.CanUserAddCarAsync(subTier, userId);
        var currentCount = await _carService.GetUserCarCountAsync(userId);

        // Get max cars for user's subscription tier
        var subscriptionTier = User.GetSubscriptionTier();
        var maxCars = subscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 1
        };

        return Ok(new
        {
            canAdd,
            currentCount,
            maxCars = maxCars == int.MaxValue ? "Unlimited" : maxCars.ToString(),
            subscriptionTier = subscriptionTier.ToString()
        });
    }
}
