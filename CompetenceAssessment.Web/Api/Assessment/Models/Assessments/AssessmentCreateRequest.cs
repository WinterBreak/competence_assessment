namespace CompetenceAssessment.Web.Assessment;

public class AssessmentCreateRequest
{
    public int TemplateId { get; }
    
    public int AssessmentTypeId { get; }
    
    public int CandidateId { get; }
    
    public List<int> InspectorsIds { get; }
}