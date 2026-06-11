using Paddokk.Core.Models.DTOs.DataExport;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.DataExport;

public static class DataExportMapping
{
    public static DataExportRequestDto ToDto(DataExportRequest request) => new(
        request.Id,
        request.Status,
        request.RequestedAt,
        request.CompletedAt,
        request.ExpiresAt);
}
