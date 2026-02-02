using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.UserManagement;

[Table("position", Schema = "security")]
public class Position
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("code")]
    public Guid Code { get; set; }
    
    [Required]
    [Column("name")]
    public string Name { get; set; }

    public virtual ICollection<User> Employees { get; set; } = [];
    
    public Position(Guid code, string name)
    {
        Code = code;
        Name = name;
    }
}