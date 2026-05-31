namespace CompetenceAssessment.Infrastructure.UserManagement;

public class Department
{
    public int Id { get; set; }
    
    public Guid Code { get; set; }
    
    public string Name { get; set; }

    public virtual ICollection<User> Employees { get; set; } = [];

    public Department() {}
    
    public Department(string name)
    {
        Code = new Guid();
        Name = name;
    }
}