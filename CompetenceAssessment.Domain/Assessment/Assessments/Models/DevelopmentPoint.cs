namespace CompetenceAssessment.Domain.Assessment;

public class DevelopmentPoint
{
    public Competence Competence { get; set; }
    
    public DateTime Date { get; set; }
    
    public decimal Score { get; set; }

    public DevelopmentPoint() {}
    
    public DevelopmentPoint(Competence competence, DateTime date, decimal score)
    {
        Competence = competence;
        Date = date;
        Score = score;
    }
}