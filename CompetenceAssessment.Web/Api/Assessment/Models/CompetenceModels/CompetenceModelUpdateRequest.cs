namespace CompetenceAssessment.Web.Assessment;

public class CompetenceModelUpdateRequest
{
    public int Id { get; }
    
    public string Name { get; }
    
    public string? Description { get; }
    
    public Dictionary<int, decimal> Weights { get;  }
}