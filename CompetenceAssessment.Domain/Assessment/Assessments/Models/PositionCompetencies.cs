using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Domain.Assessment;

public class PositionCompetencies
{
    public int PositionId { get; set; }
    
    public string PositionName { get; set; }
    
    public decimal Score { get; set; }
    
    public decimal Percentage { get; set; }
    
    public string Level { get; set; }
    
    public List<User> Employees { get; set; }
        
    public List<CompetenceResult> Competencies { get; set; }
    
    public PositionCompetencies() {}

    public PositionCompetencies(int positionId, string positionName
        , decimal score, decimal percentage, IEnumerable<CompetenceResult> competencies
        , IEnumerable<User> employees)
    {
        PositionId = positionId;
        PositionName = positionName;
        Score = score;
        Percentage = percentage;
        Competencies = competencies.ToList();
        Employees = employees.ToList();
    }
}