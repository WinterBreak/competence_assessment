using CompetenceAssessment.Domain.Export.Enumerations;

namespace CompetenceAssessment.Domain.Export.Models;

public class ReportRequest
{
    public ReportType ReportType { get; init; }
    
    protected ReportRequest(ReportType type) => ReportType = type;
}