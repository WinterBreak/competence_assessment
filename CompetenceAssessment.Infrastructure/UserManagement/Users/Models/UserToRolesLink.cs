using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.UserManagement;

[Table("users_to_roles", Schema = "security")]
public class UserToRolesLink
{
    //TODO составной PK
    
    [Required]
    [ForeignKey("user_id")]
    public int UserId { get; set; }
    
    [Required]
    [ForeignKey("role_id")]
    public int RoleId { get; set; }
    
    public virtual User User { get; set; }
    
    public virtual Role Role { get; set; }
    
    public UserToRolesLink(int userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public UserToRolesLink(User user, Role role)
    {
        User = user;
        Role = role;
    }
}