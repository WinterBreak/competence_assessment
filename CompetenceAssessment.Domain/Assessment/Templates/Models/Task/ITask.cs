namespace CompetenceAssessment.Domain.Assessment;

public class ITask
{
    public int Id { get; set; }
    
    public string Text { get; set; }
    
    public string? Answer { get; set; }
    
    public TaskType Type { get; set; } = TaskType.None;

    public ITask() {}
    
    public ITask(int id, string text, string? answer, TaskType type)
    {
        Id = id;
        Text = text;
        Answer = answer;
        Type = type;
    }
    
    public ITask(string text, string? answer, TaskType type)
    {
        Text = text;
        Answer = answer;
        Type = type;
    }
}