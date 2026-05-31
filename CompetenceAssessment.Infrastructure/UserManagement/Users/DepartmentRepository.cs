using CompetenceAssessment.Domain.UserManagement;
using Microsoft.EntityFrameworkCore;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class DepartmentRepository: IDepartmentRepository
{
    private readonly UserManagementContext _context;

    public DepartmentRepository(UserManagementContext context)
    {
        _context = context;
    }
    
    public async Task<List<Domain.UserManagement.Department>> GetAllAsync(CancellationToken token = default)
    {
        return await _context.Departments.Select(d =>
                new Domain.UserManagement.Department(d.Id, d.Name, null))
            .ToListAsync(token);
    }

    public async Task AddAsync(Domain.UserManagement.Department department, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(department);
        var newDepartment = new Department(department.Name);
        await _context.Departments.AddAsync(newDepartment, token);
    }
}