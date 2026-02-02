using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.UserManagement;

[Table("user", Schema = "security")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [ForeignKey("id_position")]
    public int PositionId { get; set; }
    
    [Required]
    [ForeignKey("id_department")]
    public int DepartmentId { get; set; }
    
    [Required]
    [Column("email")]
    [MaxLength(255)]
    public string Email { get; set; }
    
    [Required]
    [Column("first_name")]
    [MaxLength(255)]
    public string FirstName { get; set; }
    
    [Column("second_name")]
    [MaxLength(255)]
    public string SecondName { get; set; }
    
    [Required]
    [Column("last_name")]
    [MaxLength(255)]
    public string LastName { get; set; }
    
    [ForeignKey("boss_id")]
    public int BossId { get; set; }
    
    public virtual User Boss { get; set; }
    
    public virtual Position Position { get; set; }
    
    public virtual Department Department { get; set; }

    public User(string email, string firstName, string secondName, string lastName
              , User boss, Position position, Department department)
    {
        Email = email;
        FirstName = firstName;
        SecondName = secondName;
        LastName = lastName;
        Boss = boss;
        Position = position;
        Department = department;
    }
    
    public User(string email, string firstName, string secondName, string lastName
        , int bossId, int positionId, int departmentId)
    {
        Email = email;
        FirstName = firstName;
        SecondName = secondName;
        LastName = lastName;
        BossId = bossId;
        PositionId = positionId;
        DepartmentId = departmentId;
    }
}