using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompetenceAssessment.Infrastructure.Assessment;

namespace CompetenceAssessment.Infrastructure.UserManagement;

[Table("department", Schema = "security")]
public class Department
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

    public Department(Guid code, string name)
    {
        Code = code;
        Name = name;
    }
}