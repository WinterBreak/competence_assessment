namespace CompetenceAssessment.Web.Assessment;

public class TaskUpdateRequest
{
    public int Id { get; }
    
    public string Text { get; }
    
    public int TypeId { get; }
    
    public string? Answer { get; }
}