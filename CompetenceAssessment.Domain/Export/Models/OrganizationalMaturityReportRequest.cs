using CompetenceAssessment.Domain.Export.Enumerations;

namespace CompetenceAssessment.Domain.Export.Models;

public class OrganizationalMaturityReportRequest: ReportRequest
{
    public int? DepartmentId { get; init; }
        
    public OrganizationalMaturityReportRequest() : base(ReportType.OrganizationalMaturity) { }
}