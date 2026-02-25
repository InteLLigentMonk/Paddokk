using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Paddokk.Api.OpenApi;

/// <summary>
/// Adds default error responses (400, 401, 403, 404, 500) to every operation
/// based on its HTTP method, route parameters, and authorization requirements.
/// Also opts non-protected operations out of the global Bearer security requirement.
/// </summary>
public sealed class DefaultResponsesOperationTransformer : IOpenApiOperationTransformer
{
    public async Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var errorSchema = await context.GetOrCreateSchemaAsync(typeof(ApiErrorResponse), null, cancellationToken);
        context.Document?.AddComponent("ApiErrorResponse", errorSchema);

        operation.Responses ??= [];

        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        var hasAuthorize = metadata.OfType<AuthorizeAttribute>().Any();
        var hasAllowAnonymous = metadata.OfType<AllowAnonymousAttribute>().Any();
        var isProtected = hasAuthorize && !hasAllowAnonymous;

        // Opt non-protected endpoints out of the global document-level Bearer requirement
        if (!isProtected)
            operation.Security = [];

        // 500 - always possible
        operation.Responses.TryAdd("500", ErrorResponse("Internal server error - unexpected failure", context.Document));

        // 400 - mutating endpoints
        var method = context.Description.HttpMethod?.ToUpperInvariant();
        if (method is "POST" or "PUT" or "PATCH" or "DELETE")
            operation.Responses.TryAdd("400", ErrorResponse("Bad request - validation or business rule violation", context.Document));

        // 404 - endpoints with route parameters
        if (context.Description.ParameterDescriptions.Any(p => p.Source == BindingSource.Path))
            operation.Responses.TryAdd("404", ErrorResponse("Not found - the requested resource does not exist", context.Document));

        // 401/403 - protected endpoints only
        if (isProtected)
        {
            operation.Responses.TryAdd("401", ErrorResponse("Unauthorized - missing or invalid JWT token", context.Document));
            operation.Responses.TryAdd("403", ErrorResponse("Forbidden - valid token but insufficient permissions", context.Document));
        }
    }

    private static OpenApiResponse ErrorResponse(string description, OpenApiDocument? document) => new()
    {
        Description = description,
        Content = new Dictionary<string, OpenApiMediaType>
        {
            ["application/json"] = new OpenApiMediaType
            {
                Schema = new OpenApiSchemaReference("ApiErrorResponse", document)
            }
        }
    };
}
