using Microsoft.AspNetCore.Mvc;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CarsController(ICarService carService) : ControllerBase
{
    private readonly ICarService _carService = carService;

    [HttpGet("makes")]
    [EndpointSummary("Returns list of all car makes")]
    public async Task<IEnumerable<CarMakeDto>> GetCarMakes(CancellationToken cancellationToken) =>
        await _carService.GetCarMakesAsync(cancellationToken);

    [HttpGet("makes/{makeId}/models")]
    [EndpointSummary("Returns list of all car models for a specific make")]
    public async Task<IEnumerable<CarModelDto>> GetCarModels(int makeId, CancellationToken cancellationToken) => 
        await _carService.GetCarModelsByMakeAsync(makeId, cancellationToken);

    [HttpGet("models/{modelId}/generations")]
    [EndpointSummary("Returns list of all car generations for a specific model")]
    public async Task<IEnumerable<CarGenerationDto>> GetCarGenerations(int modelId, CancellationToken cancellationToken) =>
        await _carService.GetCarGenerationsByModelAsync(modelId, cancellationToken);

    [HttpGet("makes/{makeId}")]
    [EndpointSummary("Returns details of a specific car make")]
    public async Task<CarMakeDto> GetCarMake(int makeId, CancellationToken cancellationToken)=> 
        await _carService.GetCarMakeByIdAsync(makeId, cancellationToken);

    [HttpGet("models/{modelId}")]
    [EndpointSummary("Returns details of a specific car model")]
    public async Task<CarModelDto> GetCarModel(int modelId, CancellationToken cancellationToken) =>
         await _carService.GetCarModelByIdAsync(modelId, cancellationToken);

    [HttpGet("generations/{generationId}")]
    [EndpointSummary("Returns details of a specific car generation")]
    public async Task<CarGenerationDto> GetCarGeneration(int generationId, CancellationToken cancellationToken) =>
        await _carService.GetCarGenerationByIdAsync(generationId, cancellationToken);
}
