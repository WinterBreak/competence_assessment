namespace CompetenceAssessment.Domain.Assessment;

public class CreateTaskCommand
{
    public string Text { get; }

    public TaskType Type { get; }

    public Dictionary<string, bool> Answers  { get; }
    
    public CreateTaskCommand() {}

    public CreateTaskCommand(string text, TaskType type, Dictionary<string, bool> answers)
    {
        Text = text;
        Type = type;
        Answers = answers;
    }

    public ITask Create() => new ITask(Text, Type, GetAnswers(Answers));

    private List<Answer> GetAnswers(Dictionary<string, bool> answers)
    {
        var newAnswers = new List<Answer>();
        var keys = answers.Keys;
        foreach (var key in keys)
        {
            if (answers.TryGetValue(key, out var answer))
            {
                var newAnswer = new Answer(default, key, answer);
                newAnswers.Add(newAnswer);
            }
        }
        
        return newAnswers;
    }
}