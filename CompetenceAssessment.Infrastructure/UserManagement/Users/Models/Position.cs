namespace CompetenceAssessment.Infrastructure.UserManagement;

public class Position
{
    public int Id { get; set; }
    
    public Guid Code { get; set; }
    
    public string Name { get; set; }

    public virtual ICollection<User> Employees { get; set; } = [];
    
    public Position() {}
    
    public Position(Guid code, string name)
    {
        Code = code;
        Name = name;
    }
}