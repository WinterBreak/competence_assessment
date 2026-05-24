namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentQuery
{
    public List<int> Ids { get; } = [];
    
    public List<int> CandidateIds { get; } = [];
    
    public List<int> InspectoreIds { get; } = [];
    
    public AssessmentType Type { get; } = AssessmentType.None;
    
    public AssessmentState State { get; }

    public AssessmentQuery() { }
    
    public AssessmentQuery(List<int> ids = default, List<int> candidateIds = default, List<int> inspectoreIds = default
        , AssessmentType type = AssessmentType.None, AssessmentState state = AssessmentState.All)
    {
        Ids = ids;
        CandidateIds = candidateIds;
        InspectoreIds = inspectoreIds;
        Type = type;
        State = state;
    }
    
    public AssessmentQuery(int id = default, AssessmentType type = AssessmentType.None
        , AssessmentState state = AssessmentState.All)
    {
        if (id != default)
        {
            Ids.Add(id);
        }
        
        Type = type;
        State = state;
    }
}