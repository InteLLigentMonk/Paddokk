using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Follows.Commands.UnfollowUser;

public record UnfollowUserCommand(string FollowedUserId) : ICommand<Result>;
