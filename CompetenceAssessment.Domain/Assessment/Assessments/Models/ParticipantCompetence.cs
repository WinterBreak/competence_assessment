namespace CompetenceAssessment.Domain.Assessment;

public class ParticipantCompetence
{
    public Competence Competence { get; set; }
    
    public int Scale { get; set; }
    
    public decimal Score { get; set; }
    
    public decimal Percentage { get; set; }
    
    public string Level { get; set; }
    
    public ParticipantCompetence() {}

    public ParticipantCompetence(Competence competence, int scale, decimal score, decimal percentage
        , string level = default)
    {
        Competence = competence;
        Scale = scale;
        Score = score;
        Percentage = percentage;
        Level = level;
    }
}