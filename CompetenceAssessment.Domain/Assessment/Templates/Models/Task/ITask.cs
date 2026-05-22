namespace CompetenceAssessment.Domain.Assessment;

public class ITask
{
    public int Id { get; set; }
    
    public string Text { get; set; }

    public List<Answer> Answers { get; set; } = [];
    
    public TaskType Type { get; set; } = TaskType.None;

    public ITask() {}
    
    public ITask(int id, string text, TaskType type, List<Answer> answers)
    {
        Id = id;
        Text = text;
        Answers = answers;
        Type = type;
    }
    
    public ITask(string text, TaskType type, List<Answer> answers)
    {
        Text = text;
        Answers = answers;
        Type = type;
    }
}