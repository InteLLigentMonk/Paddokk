using Microsoft.OpenApi.Models;

namespace Paddokk.Api.Extensions
{
    internal class OpenApiReference
    {
        public ReferenceType Type { get; set; }
        public required string Id { get; set; }
    }
}