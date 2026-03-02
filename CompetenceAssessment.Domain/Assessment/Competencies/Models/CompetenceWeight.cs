namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceWeight
{
    public int ModelId { get; set; }
    
    public Competence Competence { get; set; }
    
    public decimal Weight { get; set; }

    public CompetenceWeight(Competence competence, int modelId, decimal weight)
    {
        Competence = competence;
        ModelId = modelId;
        Weight = weight;
    }
    
    public CompetenceWeight(Competence competence, decimal weight)
    {
        Competence = competence;
        Weight = weight;
    }
}