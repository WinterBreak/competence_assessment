namespace CompetenceAssessment.Domain.Assessment;

public class UpdateAssessmentCommand
{
    public int AssessmentId { get; }
    
    public Dictionary<int, string> Answers { get; }
    
    public Dictionary<int, int> Scores { get; }
    
    public Dictionary<int, string> Comments { get; }
    
    public string Comment { get; }

    public UpdateAssessmentCommand(int assessmentId, Dictionary<int, string> answers, Dictionary<int, int> scores
                                 , Dictionary<int, string> comments, string comment)
    {
        AssessmentId = assessmentId;
        Answers = answers;
        Scores = scores;
    }
    
    public Assessment Update(Assessment assessment)
    {
        var results = new List<AssessmentResult>();
        
        var taskIds = Answers.Keys.ToList();
        foreach (var taskId in taskIds)
        {
            var task = new ITask { Id = taskId };
            var result = new AssessmentResult(AssessmentId, task, Comments[taskId]
                                     , Answers[taskId], Scores[taskId]);
            results.Add(result);
        }
        
        assessment.Results = results;
        return assessment;
    }
}