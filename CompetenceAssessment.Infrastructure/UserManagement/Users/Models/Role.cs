using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.UserManagement;

[Table("role", Schema = "security")]
public class Role
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("name")]
    public string Name { get; set; }
    
    [Column("description")]
    public string Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = [];
    
    public Role(string description, string name)
    {
        Description = description;
        Name = name;
    }
}