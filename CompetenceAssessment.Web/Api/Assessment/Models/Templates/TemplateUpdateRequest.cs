namespace CompetenceAssessment.Web.Assessment;

public class TemplateUpdateRequest
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int Type { get; set; }
    
    public int Scale { get; set; }
    
    public int CompetenceModelId { get; set; }
    
    public Dictionary<int, decimal> Weights { get; set; }
    
    public Dictionary<int, List<int>> CompetenciesToTasks { get; set; }
}