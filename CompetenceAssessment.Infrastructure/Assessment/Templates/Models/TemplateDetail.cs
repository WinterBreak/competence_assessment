namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateDetail
{
    public int Id { get; set; }
    
    public int CompetenceId { get; set; }
    
    public int TemplateId { get; set; }
    
    public int TaskId { get; set; }
    
    public decimal Weight { get; set; }
    
    public virtual Competence Competence { get; set; }
    
    public virtual Template Template { get; set; }
    
    public virtual Task Task { get; set; }

    public TemplateDetail() { }
    
    public TemplateDetail(Template template, int taskId, int competenceId, decimal weight)
    {
        Template = template;
        TaskId = taskId;
        CompetenceId = competenceId;
        Weight = weight;
    }
    
    public TemplateDetail(int templateId, int taskId, int competenceId, decimal weight)
    {
        TemplateId = templateId;
        TaskId = taskId;
        CompetenceId = competenceId;
        Weight = weight;
    }
}