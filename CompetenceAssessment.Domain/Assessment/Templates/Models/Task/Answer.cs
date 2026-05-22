namespace CompetenceAssessment.Domain.Assessment;

public class Answer
{
    public int Id { get; set; }
    
    public int TaskId { get; set; }
    
    public string Text { get; set; }
    
    public bool IsCorrect { get; set; }
    
    public Answer() {}

    public Answer(int id, int taskId, string text, bool isCorrect)
    {
        Id = id;
        TaskId = taskId;
        Text = text;
        IsCorrect = isCorrect;
    }
    
    public Answer(string text, bool isCorrect)
    {
        Text = text;
        IsCorrect = isCorrect;
    }
}