using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.Assessment;

[Table("competence_model_detail", Schema = "assessment")]
public class CompetenceModelDetail
{
    //TODO составной PK
    //TODO отдельные конфиг. файлы для дал моделей. подсмотреть в vmo-ветке
    
    [Required]
    [ForeignKey("competence_model_id")]
    public int CompetenceModelId { get; set; }
    
    [Required]
    [ForeignKey("competence_id")]
    public int CompetenceId { get; set; }
    
    [Required]
    [Column("weight")]
    public decimal Weight { get; set; }
    
    public virtual CompetenceModel Model { get; set; }
    
    public virtual Competence Competence { get; set; }

    public CompetenceModelDetail(CompetenceModel model, Competence competence
        , decimal weight)
    {
        Model = model;
        Competence = competence;
        Weight = weight;
    }

    public CompetenceModelDetail(int modelId, int competenceId, decimal weight)
    {
        CompetenceModelId = modelId;
        CompetenceId = competenceId;
        Weight = weight;
    }
}