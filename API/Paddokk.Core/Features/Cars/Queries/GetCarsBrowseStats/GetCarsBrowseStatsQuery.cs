using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Cars.Queries.GetCarsBrowseStats;

public record GetCarsBrowseStatsQuery(
    IReadOnlyList<string> Terms,
    bool? IsPublic = null,
    string? ExcludePrincipalId = null
) : IQuery<Result<GetCarsBrowseStatsResponse>>;
