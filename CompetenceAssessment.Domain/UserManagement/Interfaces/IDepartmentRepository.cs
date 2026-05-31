namespace CompetenceAssessment.Domain.UserManagement;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync(CancellationToken token = default);
    
    Task AddAsync(Department department, CancellationToken token = default);
}