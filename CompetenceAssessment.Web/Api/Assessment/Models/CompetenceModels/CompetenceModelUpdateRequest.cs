namespace CompetenceAssessment.Web.Assessment;

public class CompetenceModelUpdateRequest
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public Dictionary<int, decimal> Weights { get; set; }
}