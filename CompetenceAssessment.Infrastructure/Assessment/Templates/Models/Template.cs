using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.Assessment;

[Table("template", Schema = "assessment")]
public class Template
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey("id_competence_model")]
    public int CompetenceModelId { get; set; }
    
    [Required]
    [ForeignKey("id_template_type")]
    public int TemplateTypeId { get; set; }
    
    [Required]
    [ForeignKey("id_scale")]
    public int ScaleId { get; set; }
    
    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; }
    
    [Required]
    [Column("creation_date")]
    public DateTime CreationDate { get; set; }

    public virtual CompetenceModel CompetenceModel { get; set; }
    
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