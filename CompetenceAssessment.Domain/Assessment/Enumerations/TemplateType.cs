using System.ComponentModel;

namespace CompetenceAssessment.Domain.Assessment;

public enum TemplateType
{
    [Description("Все")]
    None = 0,
    
    [Description("Тестирование")]
    Test = 1,
    
    [Description("Анкета")]
    Survey,
}