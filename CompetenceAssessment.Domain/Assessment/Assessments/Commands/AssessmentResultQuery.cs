namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentResultQuery
{
    public List<int> AssessmentIds { get; } = [];

    public List<int> ResultIds { get; }

    public AssessmentResultQuery(int assessmentId = default, List<int> resultIds = default)
    {
        if (assessmentId != default)
        {
            AssessmentIds.Add(assessmentId);
        }
        ResultIds = resultIds?.ToList() ?? new List<int>();
    }
    
    public AssessmentResultQuery(IEnumerable<int> assessmentIds = default, List<int> resultIds = default)
    {
        AssessmentIds = assessmentIds?.ToList() ?? new List<int>();
        ResultIds = resultIds;
    }
}