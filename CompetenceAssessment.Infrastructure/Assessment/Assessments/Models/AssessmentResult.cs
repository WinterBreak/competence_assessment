using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class AssessmentResult
{
    public int Id { get; set; }
    
    public int AssessmentId { get; set; }
    
    public int TaskId { get; set; }
    
    public string Comment { get; set; }
    
    public string Answer { get; set; }
    
    public int Score { get; set; }
    
    public virtual Assessment Assessment { get; set; }
    
    public virtual Task Task { get; set; }

    public AssessmentResult(string comment, string answer, int score
                          , Assessment assessment, Task task)
    {
        Comment = comment;
        Answer = answer;
        Score = score;
        Assessment = assessment;
        Task = task;
    }
    
    public AssessmentResult(string comment, string answer, int score
        , int assessmentId, int taskId)
    {
        Comment = comment;
        Answer = answer;
        Score = score;
        AssessmentId = assessmentId;
        TaskId = taskId;
    }
}