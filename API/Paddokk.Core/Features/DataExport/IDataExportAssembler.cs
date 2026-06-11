using Paddokk.Core.Models.DTOs.DataExport;

namespace Paddokk.Core.Features.DataExport;

/// <summary>
/// Assembles the full <see cref="DataExportDocument"/> for a user from the export reader.
/// </summary>
public interface IDataExportAssembler
{
    Task<DataExportDocument> BuildAsync(string userId, CancellationToken cancellationToken);
}
