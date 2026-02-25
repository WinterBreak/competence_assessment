namespace CompetenceAssessment.Domain.Assessment;

public class CreateCompetenceCommand
{
    public string Name { get; }
    
    public string? Description { get; }

    public CreateCompetenceCommand(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public Competence Create() => new Competence(Name, Description);
}