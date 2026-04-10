using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Web.UserManagement;

public class UserDto
{
    public int Id { get; set; }
    
    public int? BossId { get; set; }
    
    public string FirstName { get; set; }
    
    public string? SecondName { get; set; }
    
    public string LastName { get; set; }
    
    public string? BossName { get; set; }
    
    public string Email { get; set; }
    
    public int PositionId { get; set; }
    
    public string Position { get; set; }
    
    public int DepartmentId { get; set; }
    
    public string Department { get; set; }
    
    public UserDto() {}

    public UserDto(User user)
    {
        Id = user.Id;
        BossId = user.BossId;
        FirstName = user.FirstName;
        SecondName = user.SecondName;
        LastName = user.LastName;
        BossName = user.BossName;
        Email = user.Email;
        PositionId = user.PositionId;
        Position = user.Position;
        DepartmentId = user.DepartmentId;
        Department = user.Department;
    }
}