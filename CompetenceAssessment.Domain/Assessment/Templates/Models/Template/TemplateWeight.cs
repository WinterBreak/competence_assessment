namespace CompetenceAssessment.Domain.Assessment;

public class TemplateWeight
{
    public int TemplateId { get; set; }
    
    public ITask Task { get; set; }
    
    public int CompetenceId { get; set; }
    
    public decimal Weight { get; set; }

    public TemplateWeight(int templateId, ITask task, int competenceId, decimal weight)
    {
        TemplateId = templateId;
        Task = task;
        CompetenceId = competenceId;
        Weight = weight;
    }
    
    public TemplateWeight(ITask task, int competenceId, decimal weight)
    {
        Task = task;
        CompetenceId = competenceId;
        Weight = weight;
    }
}