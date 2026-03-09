namespace CompetenceAssessment.Web.Assessment;

public class TemplateUpdateRequest
{
    public int Id { get; }
    
    public string Name { get; }
    
    public int Type { get; }
    
    public int Scale { get; }
    
    public int CompetenceModelId { get;  }
    
    public Dictionary<int, decimal> Weights { get; }
    
    public Dictionary<int, List<int>> CompetenciesToTasks { get; }
}