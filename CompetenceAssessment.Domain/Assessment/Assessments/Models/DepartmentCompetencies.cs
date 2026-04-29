using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Domain.Assessment;

public class DepartmentCompetencies
{
    public int DepartmentId { get; set; }
    
    public string DepartmentName { get; set; }
    
    public decimal Score { get; set; }
    
    public decimal Percentage { get; set; }
    
    public string Level { get; set; }
    
    public List<User> Employees { get; set; }
    
    public List<CompetenceResult> Competencies { get; set; }
    
    public DepartmentCompetencies(){}

    public DepartmentCompetencies(int departmentId, string departmentName, decimal score
        , decimal percentage, List<User> employees, List<CompetenceResult> competencies)
    {
        DepartmentId = departmentId;
        DepartmentName = departmentName;
        Score = score;
        Percentage = percentage;
        Employees = employees;
        Competencies = competencies;
    }
}