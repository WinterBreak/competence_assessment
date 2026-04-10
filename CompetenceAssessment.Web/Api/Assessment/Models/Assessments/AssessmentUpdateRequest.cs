namespace CompetenceAssessment.Web.Assessment;

public class AssessmentUpdateRequest
{
    public int AssessmentId { get; set; }
    
    public Dictionary<int, string?> Answers { get; set; }
    
    public Dictionary<int, int> Scores { get; set; }
    
    public Dictionary<int, string> Comments { get; set; } // TODO перемменоввать тут и на фронте. непонятно, чем отличается от comment
    
    public string Comment { get; set; }
}