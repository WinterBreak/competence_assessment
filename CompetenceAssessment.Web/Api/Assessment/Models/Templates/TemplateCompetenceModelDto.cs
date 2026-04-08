using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class TemplateCompetenceModelDto
{
    public int ModelId { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public Dictionary<int, string> Competencies { get; set; }
    
    public TemplateCompetenceModelDto() {}

    public TemplateCompetenceModelDto(CompetenceModel model)
    {
        ModelId = model.Id;
        Name = model.Name;
        Description = model.Description;
        Competencies = model.Competencies.ToDictionary(v => v.Competence.Id
                                                     , v => v.Competence.Name);
    }
}