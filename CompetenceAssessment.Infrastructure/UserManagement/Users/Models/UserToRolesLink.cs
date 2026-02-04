namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserToRolesLink
{
    public int UserId { get; set; }
    
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