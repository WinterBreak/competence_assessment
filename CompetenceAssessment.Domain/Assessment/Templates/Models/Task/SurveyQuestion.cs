namespace CompetenceAssessment.Domain.Assessment;

public class SurveyQuestion: ITask
{
    public SurveyQuestion(int id, string text, string? answer, TaskType type) : base(id, text, answer, type)
    {
    }
}