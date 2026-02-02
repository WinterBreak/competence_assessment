using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.Assessment;

[Table("competence", Schema = "assessment")]
public class Competence
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

    public Competence(string name, string description)
    {
        Name = name;
        Description = description;
    }
}