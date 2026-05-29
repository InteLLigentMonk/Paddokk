using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Follows.Commands.FollowUser;

public record FollowUserCommand(string FollowedUserId) : ICommand<Result>;
