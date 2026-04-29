using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class CompetenceResultDto
{
    public int CompetenceId { get; set; }
    
    public string CompetenceName { get; set; }
    
    public decimal Score { get; set; }
    
    public decimal Percentage { get; set; }
    
    public string Level { get; set; }
    
    public CompetenceResultDto() {}

    public CompetenceResultDto(CompetenceResult competenceResult)
    {
        CompetenceId = competenceResult.Competence.Id;
        CompetenceName = competenceResult.Competence.Name;
        Score = competenceResult.Score;
        Percentage = competenceResult.Percentage;
        Level = competenceResult.Level;
    }
}