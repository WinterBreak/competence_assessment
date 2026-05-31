namespace CompetenceAssessment.Domain.UserManagement;

public record LoginResult
{
    public bool Succeeded { get; init; }
    public int? UserId { get; init; }
    public string? Email { get; init; }
    public string? FullName { get; init; }
    
    public List<string> Roles { get; init; }
    
    public string? ErrorMessage { get; init; }
        
    public static LoginResult Success(int userId, string email, string fullName, List<string> roles) => new()
    {
        Succeeded = true,
        UserId = userId,
        Email = email,
        FullName = fullName,
        Roles = roles
    };
        
    public static LoginResult Failed(string errorMessage) => new()
    {
        Succeeded = false,
        ErrorMessage = errorMessage
    };
}