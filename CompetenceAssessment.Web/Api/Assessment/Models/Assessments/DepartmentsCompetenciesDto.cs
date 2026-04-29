using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;

namespace CompetenceAssessment.Web.Assessment;

public class DepartmentsCompetenciesDto
{
    public int DepartmentId { get; set; }
    
    public string DepartmentName { get; set; }
    
    public List<AssessmentParticipant> Employees { get; set; }
    
    public decimal Score { get; set; }
    
    public decimal Percentage { get; set; }
    
    public string Level { get; set; }
    
    public List<CompetenceResultDto> Competencies { get; set; }

    public DepartmentsCompetenciesDto(DepartmentCompetencies competencies)
    {
        DepartmentId = competencies.DepartmentId;
        DepartmentName = competencies.DepartmentName;
        Score = competencies.Score;
        Percentage = competencies.Percentage;
        Level = competencies.Level;
        Competencies = competencies.Competencies.Select(c => new CompetenceResultDto(c)).ToList();
        Employees = competencies.Employees.Select(e => new AssessmentParticipant(e)).ToList();
    }
}