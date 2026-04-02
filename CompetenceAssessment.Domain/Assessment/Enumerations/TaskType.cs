using System.ComponentModel;

namespace CompetenceAssessment.Domain.Assessment;

public enum TaskType
{
    [Description("Не выбрано")]
    None = 0,
    
    [Description("Тестовый вопрос")]
    TestQuestion = 1,
    
    [Description("Открытый вопрос")]
    OpenQuestion = 2,
    
    [Description("Вопрос для анкеты")]
    SurveyQuestion = 3,
}