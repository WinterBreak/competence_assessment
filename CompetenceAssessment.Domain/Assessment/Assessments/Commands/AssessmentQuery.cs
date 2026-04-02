namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentQuery
{
    public List<int> Ids { get; } = [];
    
    public AssessmentType Type { get; } = AssessmentType.None;
    
    public bool IsFinished { get; } = false;

    public AssessmentQuery() { }
    
    public AssessmentQuery(List<int> ids = default, AssessmentType type = AssessmentType.None, bool isFinished = false)
    {
        Ids = ids;
        Type = type;
        IsFinished = isFinished;
    }
    
    public AssessmentQuery(int id = default, AssessmentType type = AssessmentType.None, bool isFinished = false)
    {
        if (id != default)
        {
            Ids.Add(id);
        }
        
        Type = type;
        IsFinished = isFinished;
    }
}