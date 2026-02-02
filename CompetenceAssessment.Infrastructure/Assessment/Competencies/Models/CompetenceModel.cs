using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.Assessment;

[Table("Competence_model", Schema = "assessment")]
public class CompetenceModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; }
    
    [MaxLength(500)]
    [Column("description")]
    public string Description { get; set; }
    
    [Required]
    [Column("creation_date")]
    public DateTime CreationDate { get; set; }

    public virtual ICollection<CompetenceModelDetail> Weights { get; set; } = [];
    
    public CompetenceModel(string name, string description
                         , DateTime creationDate)
    {
        Name = name;
        Description = description;
        CreationDate = creationDate;
    }
}