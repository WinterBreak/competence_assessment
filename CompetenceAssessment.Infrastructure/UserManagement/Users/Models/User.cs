using CompetenceAssessment.Infrastructure.Assessment;
using Microsoft.AspNetCore.Identity;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class User
{
    public int Id { get; set; }
    
    public int PositionId { get; set; }
    
    public int DepartmentId { get; set; }
    
    public string Email { get; set; }
    
    public string FirstName { get; set; }
    
    public string? SecondName { get; set; }
    
    public string LastName { get; set; }
    
    public string PasswordHash { get; set; }
    
    public int? BossId { get; set; }
    
    public virtual User Boss { get; set; }
    
    public virtual Position Position { get; set; }
    
    public virtual Department Department { get; set; }

    public virtual ICollection<User> Subordinates { get; set; } = [];
    
    public virtual ICollection<UserToRolesLink> RoleLinks { get; set; } = [];
    
    public virtual ICollection<AssessmentInspector> Inspections { get; set; } = [];
    
    public User() {}
    
    public User(string email, string firstName, string? secondName, string lastName
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
    
    public User(string email, string firstName, string? secondName, string lastName
        , int? bossId, int positionId, int departmentId)
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