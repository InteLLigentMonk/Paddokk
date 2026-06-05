using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Notifications.Commands.MarkNotificationRead;

/// <summary>Marks one Notification read for the authenticated actor. Idempotent.</summary>
public record MarkNotificationReadCommand(int Id) : ICommand<Result>;
