using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Infrastructure.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class TemplateWeightDto
{
    public int TemplateId { get; set; }
    
    public int TaskId { get; set; }
    
    public int Type  { get; set; }
    
    public string TaskText { get; set; }
    
    public int CompetenceId { get; set; }
    
    public decimal Weight { get; set; }
    
    public TemplateWeightDto() { }

    public TemplateWeightDto(TemplateWeight weight)
    {
        TemplateId = weight.TemplateId;
        TaskId = weight.Task.Id;
        Type = (int)weight.Task.Type;
        TaskText = weight.Task.Text;
        CompetenceId = weight.CompetenceId;
        Weight = weight.Weight;
    }
}