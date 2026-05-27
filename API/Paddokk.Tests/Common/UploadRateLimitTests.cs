using System.Net;
using System.Security.Claims;
using System.Threading.RateLimiting;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Paddokk.Tests.Common;

/// <summary>
/// Integration test for the "upload" rate-limit policy.
/// Bursts 15 requests against a minimal in-process server and asserts the 11th onwards
/// are rejected with 429 + Retry-After, while distinct users are not affected.
/// </summary>
public class UploadRateLimitTests
{
    [Fact]
    public async Task UploadPartition_AfterTenRequests_ReturnsTooManyRequestsWithRetryAfter()
    {
        using var host = await BuildHostAsync();
        var client = host.GetTestClient();

        var statuses = new List<HttpStatusCode>();
        HttpResponseMessage? rejected = null;

        for (var i = 0; i < 15; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/upload");
            request.Headers.Add("X-Test-User", "user-1");
            var response = await client.SendAsync(request);
            statuses.Add(response.StatusCode);
            if (response.StatusCode == HttpStatusCode.TooManyRequests && rejected is null)
                rejected = response;
        }

        statuses.Take(10).Should().AllSatisfy(s => s.Should().Be(HttpStatusCode.OK));
        statuses.Skip(10).Should().AllSatisfy(s => s.Should().Be(HttpStatusCode.TooManyRequests));

        rejected.Should().NotBeNull();
        rejected!.Headers.RetryAfter.Should().NotBeNull("Retry-After must be set on 429 responses");
    }

    [Fact]
    public async Task UploadPartition_DifferentUsers_DoNotShareBucket()
    {
        using var host = await BuildHostAsync();
        var client = host.GetTestClient();

        // user-1 exhausts their bucket
        for (var i = 0; i < 10; i++)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, "/upload");
            req.Headers.Add("X-Test-User", "user-1");
            await client.SendAsync(req);
        }

        // user-2 should still get their full 10
        var user2Request = new HttpRequestMessage(HttpMethod.Post, "/upload");
        user2Request.Headers.Add("X-Test-User", "user-2");
        var user2Response = await client.SendAsync(user2Request);

        user2Response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static async Task<IHost> BuildHostAsync()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddRateLimiter(options =>
                        {
                            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                            options.OnRejected = (context, _) =>
                            {
                                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                                {
                                    context.HttpContext.Response.Headers.RetryAfter =
                                        ((int)retryAfter.TotalSeconds).ToString();
                                }
                                return ValueTask.CompletedTask;
                            };

                            // Mirrors the "upload" policy in Program.cs.
                            options.AddPolicy("upload", context =>
                            {
                                var key = context.User.Identity?.IsAuthenticated == true
                                    ? context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous"
                                    : context.Request.Headers["X-Test-User"].ToString() is { Length: > 0 } header
                                        ? header
                                        : context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

                                return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                                {
                                    PermitLimit = 10,
                                    Window = TimeSpan.FromMinutes(1)
                                });
                            });
                        });
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseRateLimiter();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapPost("/upload", () => Results.Ok())
                                .RequireRateLimiting("upload");
                        });
                    });
            });

        var host = await builder.StartAsync();
        return host;
    }
}
