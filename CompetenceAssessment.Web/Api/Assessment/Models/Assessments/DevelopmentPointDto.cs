using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class DevelopmentPointDto
{
    public DateTime Date { get; set; }
    
    public decimal Score { get; set; }
    
    public int CompetenceId { get; set; }
    
    public string CompetenceName { get; set; }
    
    public DevelopmentPointDto() {}

    public DevelopmentPointDto(DevelopmentPoint point)
    {
        Date = point.Date;
        Score = point.Score;
        CompetenceId = point.Competence.Id;
        CompetenceName = point.Competence.Name;
    }
}