using API.Models.DTOs;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly ILogger<CarsController> _logger;

    public CarsController(ICarService carService, ILogger<CarsController> logger)
    {
        _carService = carService;
        _logger = logger;
    }

    // Car Database Endpoints
    [HttpGet("makes")]
    public async Task<ActionResult<IEnumerable<CarMakeDto>>> GetCarMakes()
    {
        var makes = await _carService.GetCarMakesAsync();
        return Ok(makes);
    }

    [HttpGet("makes/{makeId}/models")]
    public async Task<ActionResult<IEnumerable<CarModelDto>>> GetCarModels(int makeId)
    {
        var models = await _carService.GetCarModelsByMakeAsync(makeId);
        return Ok(models);
    }

    [HttpGet("models/{modelId}/generations")]
    public async Task<ActionResult<IEnumerable<CarGenerationDto>>> GetCarGenerations(int modelId)
    {
        var generations = await _carService.GetCarGenerationsByModelAsync(modelId);
        return Ok(generations);
    }

    [HttpGet("makes/{makeId}")]
    public async Task<ActionResult<CarMakeDto>> GetCarMake(int makeId)
    {
        var make = await _carService.GetCarMakeByIdAsync(makeId);
        if (make == null)
            return NotFound();

        return Ok(make);
    }

    [HttpGet("models/{modelId}")]
    public async Task<ActionResult<CarModelDto>> GetCarModel(int modelId)
    {
        var model = await _carService.GetCarModelByIdAsync(modelId);
        if (model == null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("generations/{generationId}")]
    public async Task<ActionResult<CarGenerationDto>> GetCarGeneration(int generationId)
    {
        var generation = await _carService.GetCarGenerationByIdAsync(generationId);
        if (generation == null)
            return NotFound();

        return Ok(generation);
    }
}
