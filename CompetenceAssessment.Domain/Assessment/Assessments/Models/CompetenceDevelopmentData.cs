using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceDevelopmentData
{
    public User Employee { get; set; }
    
    public decimal TargetScore { get; set; }
    
    public List<DevelopmentPoint> DevelopmentPoints { get; set; }
    
    public CompetenceDevelopmentData(){}
    
    public CompetenceDevelopmentData(User user, decimal targetScore, List<DevelopmentPoint> developmentPoints)
    {
        
        Employee = user;
        TargetScore = targetScore;
        DevelopmentPoints = developmentPoints;
    }
}