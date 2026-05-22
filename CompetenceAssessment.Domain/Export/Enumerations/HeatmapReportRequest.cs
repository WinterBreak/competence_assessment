using CompetenceAssessment.Domain.Export.Models;

namespace CompetenceAssessment.Domain.Export.Enumerations;

public class HeatmapReportRequest: ReportRequest
{
    public int? DepartmentId { get; init; }
    public string SortBy { get; init; }
    
    public HeatmapReportRequest() : base(ReportType.EmployeeCompetenceHeatmap) { }
}