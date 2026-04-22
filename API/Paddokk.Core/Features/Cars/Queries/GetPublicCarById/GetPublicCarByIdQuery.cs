using MediatR;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetPublicCarById;

public sealed record GetPublicCarByIdQuery(int CarId) : IRequest<Result<UserCarDto>>;
