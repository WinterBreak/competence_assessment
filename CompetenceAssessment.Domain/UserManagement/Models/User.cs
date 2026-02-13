namespace CompetenceAssessment.Domain.UserManagement;

public class User
{
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string? SecondName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public User Boss { get; set; }
    
    public int PositionId { get; set; }
    
    public int DepartmentId { get; set; }
    
    public string FullName => SecondName is null 
        ? $"{LastName} {FirstName}"
        : $"{LastName} {FirstName} {SecondName}";
    
    public User() {}

    public User(int id, string firstName, string? secondName, string lastName
              , User user, string email, int positionId, int departmentId)
    {
        Id = id;
        FirstName = firstName;
        SecondName = secondName;
        LastName = lastName;
        Email = email;
        Boss = user;
        PositionId = positionId;
        DepartmentId = departmentId;
    }
}