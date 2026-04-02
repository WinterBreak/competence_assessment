namespace CompetenceAssessment.Domain.Assessment;

public class UpdateCompetenceModelCommand
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public Dictionary<int, decimal> Weights { get; set; }

    public UpdateCompetenceModelCommand(int id, string name, string? description
        , Dictionary<int, decimal> weights)
    {
        Id = id;
        Name = name;
        Description = description;
        Weights = weights;
    }

    public void Update(CompetenceModel model)
    {
        model.Name = Name;
        model.Description = Description;
        model.Competencies = Weights.Select(
                w =>
                {
                    var competence = new Competence()
                    {
                        Id = w.Key
                    };
                    return new CompetenceWeight(competence, w.Value);
                })
            .ToList();
    }
}