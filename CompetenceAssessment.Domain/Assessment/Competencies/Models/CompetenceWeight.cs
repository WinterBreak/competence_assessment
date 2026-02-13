namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceWeight
{
    public Competence Competence { get; set; }
    
    public ITask Task { get; set; }
    
    public decimal Weight { get; set; }

    public CompetenceWeight(Competence competence, ITask task, decimal weight)
    {
        Competence = competence;
        Task = task;
        Weight = weight;
    }
}