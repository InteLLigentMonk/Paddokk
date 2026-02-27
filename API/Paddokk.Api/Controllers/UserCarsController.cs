using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Api.Extensions;
using Paddokk.Core.Features.Cars.Commands.CreateUserCar;
using Paddokk.Core.Features.Cars.Commands.DeleteUserCar;
using Paddokk.Core.Features.Cars.Commands.UpdateUserCar;
using Paddokk.Core.Features.Cars.Queries.GetUserCarById;
using Paddokk.Core.Features.Cars.Queries.GetUserCars;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/users/me/cars")]
[Authorize]
public class UserCarsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get all cars for the current user")]
    public async Task<UserCarsResponse> GetUserCars(CancellationToken ct) =>
        await sender.Send(new GetUserCarsQuery(), ct);

    [HttpGet("{carId}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Get a specific car for the current user")]
    public async Task<ActionResult<UserCarDto>> GetUserCar(int carId, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserCarByIdQuery(carId), ct);
        return OkOrError(result);
    }

    [HttpPost]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Add a new car for the current user")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserCarDto>> CreateUserCar([FromBody] CreateUserCarCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { SubscriptionTier = User.GetSubscriptionTier() }, ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return CreatedAtAction(nameof(GetUserCar), new { carId = result.Value!.Id }, result.Value);
    }

    [HttpPut("{carId}")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Update a car for the current user")]
    public async Task<ActionResult<UserCarDto>> UpdateUserCar(int carId, [FromBody] UpdateUserCarCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command with { CarId = carId }, ct);
        return OkOrError(result);
    }

    [HttpDelete("{carId}")]
    [EnableRateLimiting("writes")]
    [EndpointSummary("Delete a car for the current user")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUserCar(int carId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteUserCarCommand(carId), ct);

        if (!result.IsSuccess)
            return FromError(result.Error);

        return NoContent();
    }
}
