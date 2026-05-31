namespace CompetenceAssessment.Domain.UserManagement;

public class Department
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public int? HeadDepartment { get; set; }
    
    public Department() {}

    public Department(int id, string name, int? headDepartment = null)
    {
        Id = id;
        Name = name;
        HeadDepartment = headDepartment;
    }

    public Department(string name, int? headDepartment = null)
    {
        Name = name;
        HeadDepartment = headDepartment;
    }
}