using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CarsController(ICarService carService) : ControllerBase
{
    private readonly ICarService _carService = carService;

    // Car Database Endpoints
    [HttpGet("makes")]
    public async Task<IEnumerable<CarMakeDto>> GetCarMakes(CancellationToken cancellationToken) =>
        await _carService.GetCarMakesAsync(cancellationToken);

    [HttpGet("makes/{makeId}/models")]
    public async Task<IEnumerable<CarModelDto>> GetCarModels(int makeId, CancellationToken cancellationToken) => 
        await _carService.GetCarModelsByMakeAsync(makeId, cancellationToken);

    [HttpGet("models/{modelId}/generations")]
    public async Task<IEnumerable<CarGenerationDto>> GetCarGenerations(int modelId, CancellationToken cancellationToken) =>
        await _carService.GetCarGenerationsByModelAsync(modelId, cancellationToken);

    [HttpGet("makes/{makeId}")]
    public async Task<CarMakeDto> GetCarMake(int makeId, CancellationToken cancellationToken)=> 
        await _carService.GetCarMakeByIdAsync(makeId, cancellationToken);

    [HttpGet("models/{modelId}")]
    public async Task<CarModelDto> GetCarModel(int modelId, CancellationToken cancellationToken) =>
         await _carService.GetCarModelByIdAsync(modelId, cancellationToken);

    [HttpGet("generations/{generationId}")]
    public async Task<CarGenerationDto> GetCarGeneration(int generationId, CancellationToken cancellationToken) =>
        await _carService.GetCarGenerationByIdAsync(generationId, cancellationToken);
}
