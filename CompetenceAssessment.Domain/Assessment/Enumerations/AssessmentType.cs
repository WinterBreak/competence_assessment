using System.ComponentModel;

namespace CompetenceAssessment.Domain.Assessment;

public enum AssessmentType
{
    [Description("Все")]
    None = 0,
    
    [Description("Тестирование")]
    Testing = 1,
    
    [Description("Анкетирование")]
    Survey = 2,
    
    [Description("Оценка 360 градусов")]
    _360Degrees_ = 3
}