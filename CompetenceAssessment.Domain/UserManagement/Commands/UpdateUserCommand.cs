namespace CompetenceAssessment.Domain.UserManagement;

public class UpdateUserCommand
{
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string? SecondName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public int? BossId { get; set; }
    
    public int PositionId { get; set; }
    
    public int DepartmentId { get; set; }

    public UpdateUserCommand() { }

    public UpdateUserCommand(string firstName, string? secondName, string lastName
                           , string email, int? bossId, int positionId, int departmentId)
    {
        FirstName = firstName;
        SecondName = secondName;
        LastName = lastName;
        Email = email;
        BossId = bossId;
        PositionId = positionId;
        DepartmentId = departmentId;
    }

    public User Update() => new User(Id, FirstName, SecondName, LastName, BossId, Email, PositionId, DepartmentId);
}