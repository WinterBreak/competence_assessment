namespace CompetenceAssessment.Domain.UserManagement;

public interface IUserRepository
{
    Task<User?> GetUserAsync(UserQuery query, CancellationToken token = default);
    
    Task<List<User>> GetUsersAsync(UserQuery query, CancellationToken token = default);
    
    Task UpdateUserAsync(UpdateUserCommand command, CancellationToken token = default);
    
    Task SaveAllChangesAsync(CancellationToken token = default);
}