namespace CompetenceAssessment.Domain.Assessment;

public class TestQuestion: ITask
{
    public TestQuestion(int id, string text, string? answer, TaskType type) : base(id, text, answer, type)
    {
    }
}