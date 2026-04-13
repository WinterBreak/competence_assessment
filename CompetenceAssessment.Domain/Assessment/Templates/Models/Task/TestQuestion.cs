namespace CompetenceAssessment.Domain.Assessment;

public class TestQuestion: ITask
{
    public TestQuestion(int id, string text, TaskType type, List<Answer> answers) : base(id, text, type, answers)
    {
    }
}