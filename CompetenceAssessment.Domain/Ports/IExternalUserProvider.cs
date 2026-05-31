namespace CompetenceAssessment.Domain.UserManagement;

public interface IExternalUserProvider
{
    Task<List<ExternalUser>> GetUsersAsync(CancellationToken token = default);
}