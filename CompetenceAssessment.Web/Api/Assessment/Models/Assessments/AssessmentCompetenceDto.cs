using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class AssessmentCompetenceDto
{
    public int CompetenceId { get; set; }
    
    public string Name { get; set; }
    
    public decimal Score { get; set; }
    
    public decimal Percentage { get; set; }
    
    public string Level { get; set; }
    
    public AssessmentCompetenceDto() {}

    public AssessmentCompetenceDto(ParticipantCompetence competence)
    {
        CompetenceId = competence.Competence.Id;
        Name = competence.Competence.Name;
        Score = competence.Score;
        Percentage = competence.Percentage;
        Level = competence.Level;
    }
}