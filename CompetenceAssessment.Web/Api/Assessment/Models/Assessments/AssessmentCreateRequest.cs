namespace CompetenceAssessment.Web.Assessment;

public class AssessmentCreateRequest
{
    public int TemplateId { get; set; }
    
    public int Type { get; set; }
    
    public int CandidateId { get; set; }
    
    public List<int> InspectorsIds { get; set; }
}