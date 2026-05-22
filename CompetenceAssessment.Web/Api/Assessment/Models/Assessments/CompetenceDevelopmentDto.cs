using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class CompetenceDevelopmentDto
{
    public int EmployeeId { get; set; }
    
    public string EmployeeName { get; set; }
    
    public decimal TargetScore { get; set; }
    
    public List<DevelopmentPointDto> DevelopmentHistory { get; set; }
    
    public CompetenceDevelopmentDto() {}

    public CompetenceDevelopmentDto(CompetenceDevelopmentData data)
    {
        EmployeeId = data.Employee.Id;
        EmployeeName = data.Employee.FullName;
        TargetScore = data.TargetScore;
        DevelopmentHistory = data.DevelopmentPoints.Select(p 
            => new DevelopmentPointDto(p)).ToList();
    }
}