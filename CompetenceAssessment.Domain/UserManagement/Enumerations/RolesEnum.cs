using System.ComponentModel;

namespace CompetenceAssessment.Domain.UserManagement.Enumerations;

public enum RolesEnum
{
    [Description("Администратор")]
    Admin,
    
    [Description("Аттестуемый")]
    Candidate,
    
    [Description("Проверяющий")]
    Inspector
}