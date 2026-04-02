namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public DateTime CreationDate { get; set; }

    public virtual ICollection<CompetenceModelDetail> Weights { get; set; } = new List<CompetenceModelDetail>();
    
    public virtual ICollection<Template> Templates { get; set; } = new List<Template>();
    
    public CompetenceModel() {}
    
    public CompetenceModel(string name, string description
                         , DateTime creationDate)
    {
        Name = name;
        Description = description;
        CreationDate = creationDate;
    }
}