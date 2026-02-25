namespace CompetenceAssessment.Web.Assessment;

public class CompetenceUpdateRequest
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
}