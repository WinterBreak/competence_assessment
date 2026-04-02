namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentResult
{
    public int Id { get; set; }
    
    public int AssessmentId { get; set; }
    
    public ITask Task { get; set; }
    
    public string Comment { get; set; }
    
    public string Answer { get; set; }
    
    public int Score { get; set; }
    
    public AssessmentResult() { }

    public AssessmentResult(int id, int assessmentId, ITask task, string comment
        , string answer, int score)
    {
        Id = id;
        AssessmentId = assessmentId;
        Task = task;
        Comment = comment;
        Answer = answer;
        Score = score;
    }
    
    public AssessmentResult(int assessmentId, ITask task, string comment
                          , string answer, int score)
    {
        AssessmentId = assessmentId;
        Task = task;
        Comment = comment;
        Answer = answer;
        Score = score;
    }
}