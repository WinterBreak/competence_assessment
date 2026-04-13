namespace CompetenceAssessment.Domain.Assessment;

public class UpdateAssessmentCommand
{
    public int AssessmentId { get; }

    public Dictionary<int, string> Answers { get; } = new();

    public Dictionary<int, int> Scores { get; } = new();

    public Dictionary<int, string> Comments { get; } = new();
    
    public string Comment { get; }

    public UpdateAssessmentCommand(int assessmentId, Dictionary<int, string> answers, Dictionary<int, int> scores
                                 , Dictionary<int, string> comments, string comment)
    {
        AssessmentId = assessmentId;
        Answers = answers;
        Scores = scores;
        Comments = comments;
        Comment = comment;
    }
    
    public Assessment Update(Assessment assessment)
    {
        var results = new List<AssessmentResult>();
        
        var taskIds = Answers.Keys.ToList();
        foreach (var taskId in taskIds)
        {
            var task = new ITask { Id = taskId };
            
            Comments.TryGetValue(taskId, out var comment);
            Answers.TryGetValue(taskId, out var answer);
            Scores.TryGetValue(taskId, out var score);
            
            var result = new AssessmentResult(AssessmentId, task, comment, answer, score);
            results.Add(result);
        }
        
        assessment.Results = results;
        return assessment;
    }
}