using Paddokk.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Api.Controllers;

[ApiController]
[Route("api/users/me/cars")]
[Authorize]
public class UserCarsController(ICarService carService) : ControllerBase
{
    private readonly ICarService _carService = carService;

    [HttpGet]
    [EndpointSummary("Get all cars for the current user")]
    public async Task<UserCarsResponse> GetUserCars(CancellationToken ct) =>
        new() { Cars = [.. await _carService.GetUserCarsAsync(User.GetUserId(), ct)] };

    [HttpGet("{carId}")]
    [EndpointSummary("Get a specific car for the current user")]
    public async Task<UserCarDto> GetUserCar(int carId, CancellationToken ct) =>
        await _carService.GetUserCarByIdAsync(User.GetUserId(), carId, ct);

    [HttpPost]
    [EndpointSummary("Add a new car for the current user")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<UserCarDto> CreateUserCar([FromBody] CreateUserCarRequest request, CancellationToken ct)
    {
        var car = await _carService.CreateUserCarAsync(User.GetSubscriptionTier(), User.GetUserId(), request, ct);
        Response.StatusCode = StatusCodes.Status201Created;
        return car;
    }

    [HttpPut("{carId}")]
    [EndpointSummary("Update a car for the current user")]
    public async Task<UserCarDto> UpdateUserCar(int carId, [FromBody] UpdateUserCarRequest request, CancellationToken ct) =>
        await _carService.UpdateUserCarAsync(User.GetUserId(), carId, request, ct);

    [HttpDelete("{carId}")]
    [EndpointSummary("Delete a car for the current user")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task DeleteUserCar(int carId, CancellationToken ct)
    {
        await _carService.DeleteUserCarAsync(User.GetUserId(), carId, ct);
        Response.StatusCode = StatusCodes.Status204NoContent;
    }
}
