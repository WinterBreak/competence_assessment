namespace CompetenceAssessment.Infrastructure.Assessment;

public class Answer
{
    public int Id { get; set; }
    
    public int TaskId { get; set; }
    
    public string Text { get; set; }
    
    public bool IsCorrect { get; set; }
    
    public virtual Task Task { get; set; }

    public Answer() {}
    
    public Answer(int id, int taskId, string text, bool isCorrect)
    {
        Id = id;
        TaskId = taskId;
        Text = text;
        IsCorrect = isCorrect;
    }
    
    public Answer(int taskId, string text, bool isCorrect)
    {
        TaskId = taskId;
        Text = text;
        IsCorrect = isCorrect;
    }
    
    public Answer(Task task, string text, bool isCorrect)
    {
        Task = task;
        Text = text;
        IsCorrect = isCorrect;
    }
}