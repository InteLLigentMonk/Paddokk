using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Dashboard;

public class DashboardResponse
{
    public DashboardUser User { get; init; } = null!;
    public JourneyStatsDto Stats { get; init; } = null!;
    public DashboardLimits Limits { get; init; } = null!;
    public IEnumerable<JourneyDto> RecentJourneys { get; init; } = [];
    public JourneyDto? DefaultActiveJourney { get; init; }
    public DashboardQuickActions QuickActions { get; init; } = null!;
}

public class DashboardUser
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public SubscriptionTier SubscriptionTier { get; init; }
    public bool EmailConfirmed { get; init; }
}

public class DashboardLimits
{
    public DashboardResourceLimit Cars { get; init; } = null!;
    public DashboardResourceLimit Journeys { get; init; } = null!;
}

public class DashboardResourceLimit
{
    public int Current { get; init; }
    public string Max { get; init; } = string.Empty;
    public bool CanAdd { get; init; }
}

public class DashboardQuickActions
{
    public bool HasDefaultJourney { get; init; }
    public bool CanCreatePost { get; init; }
    public bool CanCreateJourney { get; init; }
    public bool CanAddCar { get; init; }
    public bool NeedsCarRegistration { get; init; }
}
