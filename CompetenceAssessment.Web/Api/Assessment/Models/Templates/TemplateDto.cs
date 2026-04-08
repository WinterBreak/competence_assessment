using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class TemplateDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int Type { get; set; }
    
    public int Scale { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public TemplateCompetenceModelDto CompetenceModel { get; set; }
    
    public List<TemplateWeightDto> Tasks { get; set; }
    
    public TemplateDto() {}

    public TemplateDto(ITemplate template)
    {
        Id = template.Id;
        Name = template.Name;
        Type = (int)template.Type;
        Scale = (int)template.Scale;
        CreationDate = template.CreationDate;
        CompetenceModel = new TemplateCompetenceModelDto(template.Model);
        Tasks = template.Weights.Select(w => new TemplateWeightDto(w)).ToList();
    }
}