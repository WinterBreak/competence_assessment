using System.ComponentModel;

namespace CompetenceAssessment.Domain.Assessment;

public enum AssessmentType
{
    [Description("Тестирование")]
    Testing = 1,
    
    [Description("Анкетирование")]
    Survey,
    
    [Description("Оценка 360 градусов")]
    _360Degrees_
}