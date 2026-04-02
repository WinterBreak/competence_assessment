namespace CompetenceAssessment.Web.Assessment;

public class CompetenceModelCreateRequest
{
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public Dictionary<int, decimal> Weights { get; set; }
}