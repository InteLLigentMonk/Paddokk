using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Notifications.Queries.GetUnreadCount;

/// <summary>The authenticated actor's unread Notification count, for the bell badge.</summary>
public record GetUnreadCountQuery : IQuery<Result<int>>;
