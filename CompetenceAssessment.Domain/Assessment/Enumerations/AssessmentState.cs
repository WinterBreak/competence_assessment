using System.ComponentModel;

namespace CompetenceAssessment.Domain.Assessment;

public enum AssessmentState
{
    [Description("Все")]
    All = 0,
    
    [Description("В процессе")]
    InProgress = 1,
    
    [Description("На проверке")]
    Reviewing = 2,
    
    [Description("Завершено")]
    Completed = 3,
    
    [Description("В ожидании")]
    Waiting = 4
} 