using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCarBySlug;

public record GetUserCarBySlugQuery(string Username, string Slug) : IQuery<Result<UserCarDto>>;
