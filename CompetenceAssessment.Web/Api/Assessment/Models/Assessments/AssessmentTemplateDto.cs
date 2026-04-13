using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class AssessmentTemplateDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int Type { get; set; }
    
    public int Scale { get; set; }
    
    public List<AssessmentTaskDto> Tasks { get; set; }
    
    public AssessmentTemplateDto() {}

    public AssessmentTemplateDto(ITemplate template)
    {
        Id = template.Id;
        Name = template.Name;
        Type = (int)template.Type;
        Scale = (int)template.Scale;
        Tasks = template.Weights.Select(t => new AssessmentTaskDto(t.Task)).ToList();
    }
}