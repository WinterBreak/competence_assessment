using System.ComponentModel;

namespace CompetenceAssessment.Domain.Assessment;

public enum TemplateType
{
    [Description("Тестирование")]
    Test = 1,
    
    [Description("Анкета")]
    Survey,
}