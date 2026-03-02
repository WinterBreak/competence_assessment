using System.ComponentModel.DataAnnotations;

namespace CompetenceAssessment.Domain.UserManagement;

public interface IUserValidationService
{
    Task<ValidationResult> ValidateUpdatingUserAsync(User user, CancellationToken token = default);
}