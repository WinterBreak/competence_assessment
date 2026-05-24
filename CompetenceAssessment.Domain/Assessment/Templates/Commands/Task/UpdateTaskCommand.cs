namespace CompetenceAssessment.Domain.Assessment;

public class UpdateTaskCommand(int id, string text, Dictionary<string, bool> answers, TaskType type)
{
    public int Id { get; set; } = id;

    public string Text { get; set; } = text;

    public Dictionary<string, bool> Answer { get; set; } = answers;

    public TaskType Type { get; set; } = type;

    public void Update(ITask task)
    {
        task.Id = Id;
        task.Text = Text;
        task.Answers = GetAnswers(answers);
        task.Type = Type;
    }
    
    private List<Answer> GetAnswers(Dictionary<string, bool> answers)
    {
        var newAnswers = new List<Answer>();
        var keys = answers.Keys;
        foreach (var key in keys)
        {
            if (answers.TryGetValue(key, out var answer))
            {
                var newAnswer = new Answer(Id, key, answer);
                newAnswers.Add(newAnswer);
            }
        }
        
        return newAnswers;
    }
}