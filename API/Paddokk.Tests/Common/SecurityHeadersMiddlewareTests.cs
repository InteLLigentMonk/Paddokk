using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Paddokk.Api.Middleware;

namespace Paddokk.Tests.Common;

public class SecurityHeadersMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_SetsStrictTransportSecurityHeader()
    {
        var context = await RunMiddlewareAsync();

        context.Response.Headers["Strict-Transport-Security"].ToString()
            .Should().Contain("max-age=")
            .And.Contain("includeSubDomains");
    }

    [Fact]
    public async Task InvokeAsync_SetsXContentTypeOptionsHeader()
    {
        var context = await RunMiddlewareAsync();

        context.Response.Headers["X-Content-Type-Options"].ToString()
            .Should().Be("nosniff");
    }

    [Fact]
    public async Task InvokeAsync_SetsXFrameOptionsHeader()
    {
        var context = await RunMiddlewareAsync();

        context.Response.Headers["X-Frame-Options"].ToString()
            .Should().Be("DENY");
    }

    [Fact]
    public async Task InvokeAsync_SetsReferrerPolicyHeader()
    {
        var context = await RunMiddlewareAsync();

        context.Response.Headers["Referrer-Policy"].ToString()
            .Should().Be("strict-origin-when-cross-origin");
    }

    [Fact]
    public async Task InvokeAsync_SetsContentSecurityPolicyAsReportOnly()
    {
        var context = await RunMiddlewareAsync();

        context.Response.Headers["Content-Security-Policy-Report-Only"].ToString()
            .Should().NotBeNullOrEmpty();
        context.Response.Headers.ContainsKey("Content-Security-Policy").Should().BeFalse();
    }

    [Fact]
    public async Task InvokeAsync_CallsNext()
    {
        var nextCalled = false;
        await RunMiddlewareAsync(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        nextCalled.Should().BeTrue();
    }

    private static async Task<HttpContext> RunMiddlewareAsync(RequestDelegate? next = null)
    {
        var context = new DefaultHttpContext();
        var middleware = new SecurityHeadersMiddleware(next ?? (_ => Task.CompletedTask));

        await middleware.InvokeAsync(context);

        return context;
    }
}
