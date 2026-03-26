namespace CompetenceAssessment.Domain.Assessment;

public class ITemplate
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public TemplateType Type { get; set; }
    
    public ScaleType Scale { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public int CompetenceModelId { get; set; }
    
    public List<TemplateWeight> Weights { get; set; }
    
    public ITemplate() {}
    
    public ITemplate(int id, string name, TemplateType type, ScaleType scale, DateTime creationDate
                   , int competenceModelId, List<TemplateWeight> weights)
    {
        this.Id = id;
        this.Name = name;
        this.Type = type;
        this.Scale = scale;
        this.CreationDate = creationDate;
        this.CompetenceModelId = competenceModelId;
        this.Weights = weights;
    }
    
    public ITemplate(string name, TemplateType type, ScaleType scale, DateTime creationDate
                   , int competenceModelId, List<TemplateWeight> weights)
    {
        this.Name = name;
        this.Type = type;
        this.Scale = scale;
        this.CreationDate = creationDate;
        this.CompetenceModelId = competenceModelId;
        this.Weights = weights;
    }
}