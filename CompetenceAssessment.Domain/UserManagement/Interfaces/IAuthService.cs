namespace CompetenceAssessment.Domain.UserManagement;

public interface IAuthService
{
    Task<User?> GetCurrentUserAsync(CancellationToken token = default);
    
    Task<LoginResult> LoginAsync(string email, string password, CancellationToken token = default);
    
    Task LogoutAsync(CancellationToken token = default);
}