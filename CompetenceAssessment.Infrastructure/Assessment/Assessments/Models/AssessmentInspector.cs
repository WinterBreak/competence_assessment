using CompetenceAssessment.Infrastructure.UserManagement;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class AssessmentInspector
{
    public int AssessmentId { get; set; }
    
    public int UserId { get; set; }
    
    public virtual Assessment Assessment { get; set; }
    
    public virtual User User { get; set; }

    public AssessmentInspector() {}
    
    public AssessmentInspector(int assessmentId, int userId)
    {
        AssessmentId = assessmentId;
        UserId = userId;
    }

    public AssessmentInspector(Assessment assessment, int userId)
    {
        Assessment = assessment;
        UserId = userId;
    }
}