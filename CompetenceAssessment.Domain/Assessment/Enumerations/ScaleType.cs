using System.ComponentModel;

namespace CompetenceAssessment.Domain.Assessment;

public enum ScaleType
{
    [Description("Пятизначная шкала")]
    FivePointScale = 1,
    
    [Description("Десятизначная шкала")]
    TenPointScale,
}