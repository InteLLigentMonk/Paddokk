using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Notifications.Commands.MarkAllRead;

/// <summary>Marks every unread Notification read for the authenticated actor.</summary>
public record MarkAllReadCommand : ICommand<Result>;
