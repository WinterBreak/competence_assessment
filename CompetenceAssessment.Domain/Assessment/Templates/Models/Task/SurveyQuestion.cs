namespace CompetenceAssessment.Domain.Assessment;

public class SurveyQuestion: ITask
{
    public SurveyQuestion(int id, string text, TaskType type, List<Answer> answers) : base(id, text, type, answers)
    {
    }
}