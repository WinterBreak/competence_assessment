namespace CompetenceAssessment.Domain.UserManagement;

public class User
{
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string? SecondName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public int? BossId { get; set; }
    
    public string? BossName { get; set; }
    
    public int PositionId { get; set; }
    
    public string Position { get; set; }
    
    public int DepartmentId { get; set; }
    
    public string Department { get; set; }
    
    public string FullName => SecondName is null 
        ? $"{LastName} {FirstName}"
        : $"{LastName} {FirstName} {SecondName}";
    
    public User() {}

    public User(int id, string firstName, string? secondName, string lastName
              , int? bossId, string email, int positionId, string position
              , int departmentId, string department, string? bossName)
    {
        Id = id;
        FirstName = firstName;
        SecondName = secondName;
        LastName = lastName;
        BossId = bossId;
        BossName = bossName;
        Email = email;
        PositionId = positionId;
        Position = position;
        DepartmentId = departmentId;
        Department = department;
    }
    
    public User(int id, string firstName, string? secondName, string lastName
        , int? bossId, string email, int positionId, int departmentId)
    {
        Id = id;
        FirstName = firstName;
        SecondName = secondName;
        LastName = lastName;
        BossId = bossId;
        Email = email;
        PositionId = positionId;
        DepartmentId = departmentId;
    }
}