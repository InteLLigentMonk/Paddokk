using System.Reflection;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Paddokk.Api.Controllers;

namespace Paddokk.Tests.Features.Comments.Controllers;

public class ReportCommentEndpointTests
{
    [Fact]
    public void CommentsController_HasAuthorizeAttribute_So_UnauthenticatedRequestsReceive401()
    {
        var authorize = typeof(CommentsController).GetCustomAttribute<AuthorizeAttribute>(inherit: true);

        authorize.Should().NotBeNull(
            "the [Authorize] attribute on CommentsController is what forces 401 for unauthenticated callers to /report");
    }

    [Fact]
    public void ReportComment_Returns501_WithMessageDetail()
    {
        var sender = Substitute.For<ISender>();
        var controller = new CommentsController(sender);

        var result = controller.ReportComment(commentId: 42, reason: "spam");

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status501NotImplemented);

        var problem = objectResult.Value.Should().BeAssignableTo<ProblemDetails>().Subject;
        problem.Status.Should().Be(StatusCodes.Status501NotImplemented);
        problem.Detail.Should().NotBeNullOrWhiteSpace(
            "the frontend reads ProblemDetails.detail to render the 'moderation coming soon' notice");
    }
}
