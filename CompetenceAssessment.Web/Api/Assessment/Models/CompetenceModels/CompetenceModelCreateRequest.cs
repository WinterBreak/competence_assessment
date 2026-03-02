namespace CompetenceAssessment.Web.Assessment;

public class CompetenceModelCreateRequest
{
    public string Name { get; }
    
    public string? Description { get; }
    
    public Dictionary<int, decimal> Weights { get; }
}