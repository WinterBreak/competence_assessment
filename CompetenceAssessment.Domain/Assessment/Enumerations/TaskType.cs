using System.ComponentModel;

namespace CompetenceAssessment.Domain.Assessment;

public enum TaskType
{
    [Description("Тестовый вопрос")]
    TestQuestion = 1,
    
    [Description("Открытый вопрос")]
    OpenQuestion,
    
    [Description("Вопрос для анкеты")]
    SurveyQuestion,
}