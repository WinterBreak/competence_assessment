namespace CompetenceAssessment.Domain.Assessment;

public class UpdateTaskCommand(int id, string text, string? answer, TaskType type)
{
    public int Id { get; set; } = id;

    public string Text { get; set; } = text;

    public string? Answer { get; set; } = answer;

    public TaskType Type { get; set; } = type;

    public void Update(ITask task)
    {
        task.Id = Id;
        task.Text = Text;
        task.Answer = Answer;
        task.Type = Type;
    }
}