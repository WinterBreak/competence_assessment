namespace CompetenceAssessment.Domain.Assessment;

public class CreateTaskCommand(string text, TaskType type, string? answer = null)
{
    public string Text { get; } = text;

    public TaskType Type { get; } = type;

    public string? Answer  { get; } = answer;

    public ITask Create() => new ITask(Text, Answer, Type);
}