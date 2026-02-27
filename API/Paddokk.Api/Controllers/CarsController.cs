using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Paddokk.Core.Features.Cars.Queries.GetCarGenerationById;
using Paddokk.Core.Features.Cars.Queries.GetCarGenerationsByModel;
using Paddokk.Core.Features.Cars.Queries.GetCarMakeById;
using Paddokk.Core.Features.Cars.Queries.GetCarMakes;
using Paddokk.Core.Features.Cars.Queries.GetCarModelById;
using Paddokk.Core.Features.Cars.Queries.GetCarModelsByMake;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Api.Controllers;

[ApiVersion(1)]
[Route("api/v{v:apiVersion}/[controller]")]
public class CarsController(ISender sender) : ApiControllerBase
{
    [HttpGet("makes")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Returns list of all car makes")]
    public async Task<CarMakesResponse> GetCarMakes(CancellationToken ct) =>
        await sender.Send(new GetCarMakesQuery(), ct);

    [HttpGet("makes/{makeId}/models")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Returns list of all car models for a specific make")]
    public async Task<CarModelsResponse> GetCarModels(int makeId, CancellationToken ct) =>
        await sender.Send(new GetCarModelsByMakeQuery(makeId), ct);

    [HttpGet("models/{modelId}/generations")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Returns list of all car generations for a specific model")]
    public async Task<CarGenerationsResponse> GetCarGenerations(int modelId, CancellationToken ct) =>
        await sender.Send(new GetCarGenerationsByModelQuery(modelId), ct);

    [HttpGet("makes/{makeId}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Returns details of a specific car make")]
    public async Task<ActionResult<CarMakeDto>> GetCarMake(int makeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCarMakeByIdQuery(makeId), ct);
        return OkOrError(result);
    }

    [HttpGet("models/{modelId}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Returns details of a specific car model")]
    public async Task<ActionResult<CarModelDto>> GetCarModel(int modelId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCarModelByIdQuery(modelId), ct);
        return OkOrError(result);
    }

    [HttpGet("generations/{generationId}")]
    [EnableRateLimiting("reads")]
    [EndpointSummary("Returns details of a specific car generation")]
    public async Task<ActionResult<CarGenerationDto>> GetCarGeneration(int generationId, CancellationToken ct)
    {
        var result = await sender.Send(new GetCarGenerationByIdQuery(generationId), ct);
        return OkOrError(result);
    }
}
