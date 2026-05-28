using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;

namespace Paddokk.Core.Features.Cars.Queries.GetUserCarsByUsername;

public record GetUserCarsByUsernameQuery(string Username, int? Limit = null) : IQuery<Result<IEnumerable<UserCarDto>>>;
