using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.UserManagement;

public interface IUserService
{
    Task<User?> GetUserAsync(UserQuery query, CancellationToken token = default);
    
    Task<List<User>> GetUsersAsync(UserQuery query, CancellationToken token = default);
    
    Task<ValidationErrors> UpdateUserAsync(UpdateUserCommand command, CancellationToken token = default);
    
    // TODO Delete
}