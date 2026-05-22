using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class AssessmentResultDto
{
    public int Id { get; set; }
    
    public int AssessmentId { get; set; }
    
    public int TaskId { get; set; }
    
    public string? Comment { get; set; }
    
    public string Answer { get; set; }
    
    public int Score { get; set; }
    
    public AssessmentResultDto() {}

    public AssessmentResultDto(AssessmentResult result)
    {
        Id = result.Id;
        AssessmentId = result.AssessmentId;
        TaskId = result.Task.Id;
        Comment = result.Comment;
        Answer = result.Answer;
        Score = result.Score;
    }
}