namespace CompetenceAssessment.Infrastructure.Assessment;

public class Template
{
    public int Id { get; set; }
    
    public int CompetenceModelId { get; set; }
    
    public int TemplateTypeId { get; set; }
    
    public int ScaleId { get; set; }
    
    public string Name { get; set; }
    
    public DateTime CreationDate { get; set; }

    public virtual CompetenceModel CompetenceModel { get; set; }
    
    public virtual ICollection<TemplateDetail> TemplateDetails { get; set; }
    
    public Template() {}
    
    public Template(CompetenceModel competenceModel, int typeId
        , int scaleId, string name, DateTime creationDate)
    {
        CompetenceModel = competenceModel;
        TemplateTypeId = typeId;
        ScaleId = scaleId;
        Name = name;
        CreationDate = creationDate;
    }
    
    public Template(int competenceModelId, int typeId
        , int scaleId, string name, DateTime creationDate)
    {
        CompetenceModelId = competenceModelId;
        TemplateTypeId = typeId;
        ScaleId = scaleId;
        Name = name;
        CreationDate = creationDate;
    }
}