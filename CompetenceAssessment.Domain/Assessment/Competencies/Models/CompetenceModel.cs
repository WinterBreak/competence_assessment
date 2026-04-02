namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public List<CompetenceWeight> Competencies { get; set; }
    
    public CompetenceModel() {}

    public CompetenceModel(int id, string name, string? description
        , DateTime creationDate, List<CompetenceWeight> competences)
    {
        Id = id;
        Name = name;
        Description = description;
        CreationDate = creationDate;
        Competencies = competences;
    }
    
    public CompetenceModel(string name, string? description
        , DateTime creationDate, List<CompetenceWeight> competences)
    {
        Name = name;
        Description = description;
        CreationDate = creationDate;
        Competencies = competences;
    }
}