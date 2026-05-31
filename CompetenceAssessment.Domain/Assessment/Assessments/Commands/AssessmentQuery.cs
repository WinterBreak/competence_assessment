namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentQuery
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public List<int> Ids { get; } = [];
    
    public List<int> CandidateIds { get; } = [];
    
    public List<int> InspectoreIds { get; } = [];
    
    public AssessmentType Type { get; } = AssessmentType.None;
    
    public AssessmentState State { get; }
    
    public bool WithChildren { get; }

    public AssessmentQuery(bool withChildren = true, int page = 1, int pageSize = 10)
    {
        Page = page;
        PageSize = pageSize;
    }
    
    public AssessmentQuery(List<int> ids = default, List<int> candidateIds = default, List<int> inspectoreIds = default
        , AssessmentType type = AssessmentType.None, AssessmentState state = AssessmentState.All, bool withChildren = true
        , int page = 1, int pageSize = 10)
    {
        Ids = ids;
        CandidateIds = candidateIds;
        InspectoreIds = inspectoreIds;
        Type = type;
        State = state;
        WithChildren = withChildren;
        Page = page;
        PageSize = pageSize;
    }
    
    public AssessmentQuery(int id = default, AssessmentType type = AssessmentType.None
        , AssessmentState state = AssessmentState.All, bool withChildren = true
        , int page = 1, int pageSize = 10)
    {
        if (id != default)
        {
            Ids.Add(id);
        }
        
        Type = type;
        State = state;
        WithChildren = withChildren;
        Page = page;
        PageSize = pageSize;
    }
}