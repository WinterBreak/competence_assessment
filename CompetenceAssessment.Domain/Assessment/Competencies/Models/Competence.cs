namespace CompetenceAssessment.Domain.Assessment;

public class Competence
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }

    public Competence() { }

    public Competence(int id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
    
    public Competence(string name, string? description)
    {
        Name = name;
        Description = description;
    }
}