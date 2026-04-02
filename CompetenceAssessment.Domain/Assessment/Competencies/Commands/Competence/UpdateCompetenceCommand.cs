namespace CompetenceAssessment.Domain.Assessment;

public class UpdateCompetenceCommand
{
    public int Id { get; }
    
    public string Name { get; }
    
    public string? Description { get; }

    public UpdateCompetenceCommand(int id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public void Update(Competence competence)
    {
        competence.Name = Name;
        competence.Description = Description;
    }
}