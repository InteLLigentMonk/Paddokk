using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    // Car Database Endpoints
    [HttpGet("makes")]
    public async Task<ActionResult<IEnumerable<CarMakeDto>>> GetCarMakes(CancellationToken cancellationToken)
    {
        var makes = await _carService.GetCarMakesAsync(cancellationToken);
        return Ok(makes);
    }

    [HttpGet("makes/{makeId}/models")]
    public async Task<ActionResult<IEnumerable<CarModelDto>>> GetCarModels(int makeId, CancellationToken cancellationToken)
    {
        var models = await _carService.GetCarModelsByMakeAsync(makeId, cancellationToken);
        return Ok(models);
    }

    [HttpGet("models/{modelId}/generations")]
    public async Task<ActionResult<IEnumerable<CarGenerationDto>>> GetCarGenerations(int modelId, CancellationToken cancellationToken)
    {
        var generations = await _carService.GetCarGenerationsByModelAsync(modelId, cancellationToken);
        return Ok(generations);
    }

    [HttpGet("makes/{makeId}")]
    public async Task<ActionResult<CarMakeDto>> GetCarMake(int makeId, CancellationToken cancellationToken)
    {
        var make = await _carService.GetCarMakeByIdAsync(makeId, cancellationToken);
        if (make == null)
            return NotFound();

        return Ok(make);
    }

    [HttpGet("models/{modelId}")]
    public async Task<ActionResult<CarModelDto>> GetCarModel(int modelId, CancellationToken cancellationToken)
    {
        var model = await _carService.GetCarModelByIdAsync(modelId, cancellationToken);
        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("generations/{generationId}")]
    public async Task<ActionResult<CarGenerationDto>> GetCarGeneration(int generationId, CancellationToken cancellationToken)
    {
        var generation = await _carService.GetCarGenerationByIdAsync(generationId, cancellationToken);
        if (generation == null)
            return NotFound();

        return Ok(generation);
    }
}
