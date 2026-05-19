using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Queries.GetCarsBrowseStats;

public sealed class GetCarsBrowseStatsHandler(ICarRepository carRepository, IActorResolver actor)
    : IRequestHandler<GetCarsBrowseStatsQuery, Result<GetCarsBrowseStatsResponse>>
{
    public async Task<Result<GetCarsBrowseStatsResponse>> Handle(GetCarsBrowseStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await carRepository.GetBrowseStatsAsync(
            request.Terms,
            request.IsPublic,
            actor.IsAuthenticated ? actor.UserId : null,
            cancellationToken);
        return Result<GetCarsBrowseStatsResponse>.Success(stats);
    }
}
