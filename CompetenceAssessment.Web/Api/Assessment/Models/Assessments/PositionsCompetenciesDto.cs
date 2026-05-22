using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;

namespace CompetenceAssessment.Web.Assessment;

public class PositionsCompetenciesDto
{
    public int PositionId { get; set; }
    
    public string PositionName { get; set; }
    
    public List<AssessmentParticipant> Employees { get; set; }
    
    public decimal Score { get; set; }
    
    public decimal Percentage { get; set; }
    
    public string Level { get; set; }
    
    public List<CompetenceResultDto> Competencies { get; set; }

    public PositionsCompetenciesDto(PositionCompetencies competencies)
    {
        PositionId = competencies.PositionId;
        PositionName = competencies.PositionName;
        Score = competencies.Score;
        Percentage = competencies.Percentage;
        Level = competencies.Level;
        Competencies = competencies.Competencies.Select(c => new CompetenceResultDto(c)).ToList();
        Employees = competencies.Employees.Select(e => new AssessmentParticipant(e)).ToList();
    }
}