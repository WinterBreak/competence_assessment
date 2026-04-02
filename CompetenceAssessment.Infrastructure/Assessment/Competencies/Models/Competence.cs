namespace CompetenceAssessment.Infrastructure.Assessment;

public class Competence
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }

    public virtual ICollection<CompetenceModelDetail> CompetenceModelDetails { get; set; }
    
    public Competence() {}
    
    public Competence(string name, string description)
    {
        Name = name;
        Description = description;
    }
}