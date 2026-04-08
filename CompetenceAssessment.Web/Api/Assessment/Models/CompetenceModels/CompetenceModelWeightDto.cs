using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class CompetenceModelWeightDto
{
    public int ModelId { get; set; }
    
    public int CompetenceId { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public decimal Weight { get; set; }
    
    public CompetenceModelWeightDto() {}

    public CompetenceModelWeightDto(CompetenceWeight from)
    {
        ModelId = from.ModelId;
        CompetenceId = from.Competence.Id;
        Name = from.Competence.Name;
        Description = from.Competence.Description;
        Weight = from.Weight;
    }
}