using CompetenceAssessment.Domain.Export.Models;

namespace CompetenceAssessment.Domain.Export;

public interface IReportExportService
{
    Task<byte[]> GenerateReportAsync(ReportRequest request, CancellationToken token = default);
}