namespace CompetenceAssessment.Domain.UserManagement;

public class Position
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public Position() {}

    public Position(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Position(string name)
    {
        Name = name;
    }
}