using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Paddokk.Api.Filters;

/// <summary>
/// Matches the JSON shape returned by GlobalExceptionMiddleware.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>Human-readable error message.</summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>HTTP status code.</summary>
    public int StatusCode { get; set; }

    /// <summary>Request path that triggered the error.</summary>
    public string Path { get; set; } = string.Empty;
}

public class DefaultResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var errorSchema = context.SchemaGenerator.GenerateSchema(
            typeof(ApiErrorResponse), context.SchemaRepository);

        var errorContent = new Dictionary<string, OpenApiMediaType>
        {
            ["application/json"] = new OpenApiMediaType { Schema = errorSchema }
        };

        // 500 — always possible
        operation.Responses.TryAdd("500", new OpenApiResponse
        {
            Description = "Internal server error — unexpected failure",
            Content = new Dictionary<string, OpenApiMediaType>(errorContent)
        });

        // 503 — request cancelled (CancellationToken)
        operation.Responses.TryAdd("503", new OpenApiResponse
        {
            Description = "Service unavailable — request was cancelled",
            Content = new Dictionary<string, OpenApiMediaType>(errorContent)
        });

        // 400 — mutating endpoints can fail validation / business rules
        var method = context.ApiDescription.HttpMethod?.ToUpperInvariant();
        if (method is "POST" or "PUT" or "PATCH" or "DELETE")
        {
        operation.Responses.TryAdd("400", new OpenApiResponse
        {
            Description = "Bad request — validation or business rule violation",
            Content = new Dictionary<string, OpenApiMediaType>(errorContent)
        });
        }

        // 404 — endpoints with route parameters can return not found
        var hasRouteParams = context.ApiDescription.ParameterDescriptions
            .Any(p => p.Source.Id == "Path");

        if (hasRouteParams)
        {
            operation.Responses.TryAdd("404", new OpenApiResponse
            {
                Description = "Not found — the requested resource does not exist",
                Content = new Dictionary<string, OpenApiMediaType>(errorContent)
            });
        }

        // 401 / 403 — only on endpoints that require auth
        var hasAuthorize = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any() || 
            (context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any() ?? false);

        var hasAllowAnonymous = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (hasAuthorize && !hasAllowAnonymous)
        {
            operation.Responses.TryAdd("401", new OpenApiResponse
            {
                Description = "Unauthorized — missing or invalid JWT token"
            });

            operation.Responses.TryAdd("403", new OpenApiResponse
            {
                Description = "Forbidden — valid token but insufficient permissions"
            });
        }
    }
}