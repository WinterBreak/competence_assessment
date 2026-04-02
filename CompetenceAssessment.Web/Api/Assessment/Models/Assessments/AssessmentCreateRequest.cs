namespace CompetenceAssessment.Web.Assessment;

public class AssessmentCreateRequest
{
    public int TemplateId { get; set; }
    
    public int AssessmentTypeId { get; set; }
    
    public int CandidateId { get; set; }
    
    public List<int> InspectorsIds { get; set; }
}