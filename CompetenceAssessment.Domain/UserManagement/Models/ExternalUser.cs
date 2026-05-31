namespace CompetenceAssessment.Domain.UserManagement;

public class ExternalUser
{
    public string Email { get; set; }
    
    public string FirstName { get; set; }
    
    public string? SecondName { get; set; }
    
    public string LastName { get; set; }
    
    public string PositionName { get; set; }
    
    public string DepartmentName { get; set; }
    
    public string? BossEmail { get; set; }
}