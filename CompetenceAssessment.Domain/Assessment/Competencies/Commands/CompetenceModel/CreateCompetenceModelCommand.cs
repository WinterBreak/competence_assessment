namespace CompetenceAssessment.Domain.Assessment;

public class CreateCompetenceModelCommand
{
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public Dictionary<int, decimal> Weights { get; set; }

    public CreateCompetenceModelCommand(string name, Dictionary<int, decimal> weights
        , string? description = null)
    {
        Name = name;
        Description = description;
        Weights = weights;
    }

    public CompetenceModel Create()
    {
        var weights = Weights.Select(
                w =>
                {
                    var competence = new Competence()
                    {
                        Id = w.Key
                    };
                    return new CompetenceWeight(competence, w.Value);
                })
            .ToList();
        
        return new CompetenceModel(Name, Description, DateTime.UtcNow, weights);
    }
}