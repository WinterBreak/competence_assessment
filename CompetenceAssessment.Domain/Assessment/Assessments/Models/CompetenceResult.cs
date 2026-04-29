namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceResult
{
    public Competence Competence { get; set; }
    
    public decimal Score { get; set; }
    
    public decimal Percentage { get; set; }
    
    public string Level { get; set; }
    
    public CompetenceResult() {}

    public CompetenceResult(Competence competence, decimal score, decimal percentage)
    {
        Competence = competence;
        Score = score;
        Percentage = percentage;
    }
}