namespace CompetenceAssessment.Web.Assessment;

public class AssessmentUpdateRequest
{
    public int AssessmentId { get; }
    
    public Dictionary<int, string> Answers { get; }
    
    public Dictionary<int, int> Scores { get; }
    
    public Dictionary<int, string> Comments { get; }
    
    public string Comment { get; }
}