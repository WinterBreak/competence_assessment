namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentResultQuery
{
    public int AssessmentId { get; }

    public List<int> ResultIds { get; } = [];

    public AssessmentResultQuery(int assessmentId = default, List<int> resultIds = default)
    {
        AssessmentId = assessmentId;
        ResultIds = resultIds;
    }
}