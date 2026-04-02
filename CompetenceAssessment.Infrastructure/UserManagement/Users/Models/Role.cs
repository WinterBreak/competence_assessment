namespace CompetenceAssessment.Infrastructure.UserManagement;

public class Role
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }

    public virtual ICollection<UserToRolesLink> UserLinks { get; set; } = [];
    
    public Role() {}
    
    public Role(string description, string name)
    {
        Description = description;
        Name = name;
    }
}