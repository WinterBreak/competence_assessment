using CompetenceAssessment.Domain.Export.Enumerations;

namespace CompetenceAssessment.Domain.Export.Models;

public class PositionMatrixReportRequest : ReportRequest
{
    public int? PositionId { get; init; }
    
    public PositionMatrixReportRequest() : base(ReportType.PositionMatrix) { }
}