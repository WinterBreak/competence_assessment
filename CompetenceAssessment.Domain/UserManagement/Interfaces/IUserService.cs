using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.UserManagement;

public interface IUserService
{
    Task<User?> GetUserAsync(UserQuery query, CancellationToken cancellationToken = default);
    
    Task<List<User>> GetUsersAsync(UserQuery query, CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken = default);
}