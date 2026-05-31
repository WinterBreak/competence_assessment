namespace CompetenceAssessment.Domain.UserManagement;

public interface IPositionRepository
{
    Task<List<Position>> GetAllAsync(CancellationToken token = default);
    
    Task AddAsync(Position position, CancellationToken token = default);
}