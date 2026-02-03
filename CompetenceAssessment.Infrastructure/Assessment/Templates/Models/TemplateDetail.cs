namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateDetail
{
    public int Id { get; set; }
    
    public int TemplateId { get; set; }
    
    public int TaskId { get; set; }
    
    public decimal Weight { get; set; }
    
    public virtual Template Template { get; set; }
    
    public virtual Task Task { get; set; }

    public TemplateDetail(Template template, Task task, decimal weight)
    {
        Template = template;
        Task = task;
        Weight = weight;
    }
    
    public TemplateDetail(int templateId, int taskId, decimal weight)
    {
        TemplateId = templateId;
        TaskId = taskId;
        Weight = weight;
    }
}