using System.ComponentModel.DataAnnotations;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.UserManagement;

public interface IUserValidationService
{
    Task<ValidationErrors> ValidateUpdatingUserAsync(User user, CancellationToken token = default);
}