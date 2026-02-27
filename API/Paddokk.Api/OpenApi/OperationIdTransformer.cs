using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Paddokk.Api.OpenApi;

public class OperationIdTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var endpointName = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<IEndpointNameMetadata>()
            .FirstOrDefault()?.EndpointName;

        if (!string.IsNullOrEmpty(endpointName))
        {
            operation.OperationId = endpointName;
            return Task.CompletedTask;
        }

        if (context.Description.ActionDescriptor is ControllerActionDescriptor descriptor)
            operation.OperationId = $"{descriptor.ControllerName}_{descriptor.ActionName}";

        return Task.CompletedTask;
    }
}
