using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class CompetenceModelGetDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public List<CompetenceModelWeightDto> Competencies { get; set; }
    
    public CompetenceModelGetDto() {}

    public CompetenceModelGetDto(CompetenceModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Description = model.Description;
        CreationDate = model.CreationDate;
        Competencies = model.Competencies.Select(c => new CompetenceModelWeightDto(c)).ToList();;
    }
}