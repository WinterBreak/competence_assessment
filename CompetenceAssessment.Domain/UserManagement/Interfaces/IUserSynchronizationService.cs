namespace CompetenceAssessment.Domain.UserManagement;

public interface IUserSynchronizationService
{
    Task<SynchronizationResult> SynchronizeAsync(CancellationToken token = default);
}