using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompetenceAssessment.Infrastructure.Assessment;

[Table("assessment_result_detail", Schema = "assessment")]
public class AssessmentResult
{
    //TODO cоставной PK
    
    [Required]
    [ForeignKey("id_assessment_result")]
    public int AssessmentId { get; set; }
    
    [Required]
    [ForeignKey("task_id")]
    public int TaskId { get; set; }
    
    [Column("comment")]
    public string Comment { get; set; }
    
    [Column("answer")]
    public string Answer { get; set; }
    
    [Required]
    [Column("score")]
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