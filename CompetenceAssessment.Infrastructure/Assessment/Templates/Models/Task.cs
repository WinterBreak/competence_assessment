namespace CompetenceAssessment.Infrastructure.Assessment;

public class Task
{
    public int Id { get; set; }
    
    public int TaskTypeId { get; set; }
    
    public string Text { get; set; }
    
    public string? Answer { get; set; }
    
    public virtual ICollection<AssessmentResult> Results { get; set; } = [];
    
    public virtual ICollection<TemplateDetail> TemplateDetails { get; set; } = [];

    public Task(int typeId, string text, string? answer)
    {
        TaskTypeId = typeId;
        Text = text;
        Answer = answer;
    }
}