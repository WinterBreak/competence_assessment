namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentQuery
{
    public List<int> Ids { get; } = [];
    
    public List<int> CandidateIds { get; } = [];
    
    public List<int> InspectoreIds { get; } = [];
    
    public AssessmentType Type { get; } = AssessmentType.None;
    
    public AssessmentState State { get; }
    
    public bool WithChildren { get; }

    public AssessmentQuery(bool withChildren = true) { }
    
    public AssessmentQuery(List<int> ids = default, List<int> candidateIds = default, List<int> inspectoreIds = default
        , AssessmentType type = AssessmentType.None, AssessmentState state = AssessmentState.All, bool withChildren = true)
    {
        Ids = ids;
        CandidateIds = candidateIds;
        InspectoreIds = inspectoreIds;
        Type = type;
        State = state;
        WithChildren = withChildren;
    }
    
    public AssessmentQuery(int id = default, AssessmentType type = AssessmentType.None
        , AssessmentState state = AssessmentState.All, bool withChildren = true)
    {
        if (id != default)
        {
            Ids.Add(id);
        }
        
        Type = type;
        State = state;
        WithChildren = withChildren;
    }
}