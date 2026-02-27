using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Dashboard;

public record GetDashboardQuery : IQuery<Result<DashboardResponse>>;
