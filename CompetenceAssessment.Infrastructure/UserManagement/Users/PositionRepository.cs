using CompetenceAssessment.Domain.UserManagement;
using Microsoft.EntityFrameworkCore;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class PositionRepository: IPositionRepository
{
    private readonly UserManagementContext _context;

    public PositionRepository(UserManagementContext context)
    {
        _context = context;
    }
    
    public async Task<List<Domain.UserManagement.Position>> GetAllAsync(CancellationToken token = default)
    {
        return await _context.Positions.Select(p =>
                new Domain.UserManagement.Position(p.Id, p.Name))
            .ToListAsync(token);
    }

    public async Task AddAsync(Domain.UserManagement.Position position, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(position);
        var newPosition = new Position(position.Name);
        await _context.Positions.AddAsync(newPosition, token);
    }
}