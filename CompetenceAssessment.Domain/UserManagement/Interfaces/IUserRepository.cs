namespace CompetenceAssessment.Domain.UserManagement;

public interface IUserRepository
{
    Task<User?> GetUserAsync(UserQuery query, CancellationToken token = default);
    
    Task<List<User>> GetUsersAsync(UserQuery query, CancellationToken token = default);
    
    Task AddUserAsync(User user, CancellationToken token = default);
    
    Task UpdateUserAsync(User user, CancellationToken token = default);
    
    Task RemoveUserAsync(int id, CancellationToken token = default);
    
    Task<bool> ValidatePasswordAsync(int userId, string password, CancellationToken token = default);
    
    Task SaveAllChangesAsync(CancellationToken token = default);
    
    Task SaveChangesWithAsssessment(CancellationToken token = default);
}