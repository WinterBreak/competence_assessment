namespace CompetenceAssessment.Domain.UserManagement;

public class UpdateUserCommand
{
    public int Id { get; set; }
    
    public string FirstName { get; }
    
    public string? SecondName { get; }
    
    public string LastName { get; }
    
    public string Email { get; }
    
    public int? BossId { get; }
    
    public int PositionId { get; }
    
    public int DepartmentId { get; }
    
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