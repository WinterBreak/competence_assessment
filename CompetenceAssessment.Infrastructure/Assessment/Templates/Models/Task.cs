namespace CompetenceAssessment.Infrastructure.Assessment;

public class Task
{
    public int Id { get; set; }
    
    public int TaskTypeId { get; set; }
    
    public string Text { get; set; }
    
    public virtual ICollection<AssessmentResult> Results { get; set; } = [];
    
    public virtual ICollection<TemplateDetail> TemplateDetails { get; set; } = [];
    
    public virtual ICollection<Answer> Answers { get; set; } = [];

    public Task() {}
    
    public Task(int typeId, string text)
    {
        TaskTypeId = typeId;
        Text = text;
    }
}