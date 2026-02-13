namespace CompetenceAssessment.Domain.UserManagement;

public interface IUserRepository
{
    Task<User?> GetUserAsync(UserQuery query, CancellationToken cancellationToken = default);
    
    Task<List<User>> GetUsersAsync(UserQuery query, CancellationToken cancellationToken = default);
    
    Task UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken = default);
    
    Task SaveAllChangesAsync(CancellationToken cancellationToken = default);
}