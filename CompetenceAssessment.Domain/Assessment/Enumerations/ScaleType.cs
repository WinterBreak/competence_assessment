using System.ComponentModel;

namespace CompetenceAssessment.Domain.Assessment;

public enum ScaleType
{
    [Description("Пятизначная шкала")]
    FivePointScale = 5,
    
    [Description("")]
    TenPointScale = 10,
}