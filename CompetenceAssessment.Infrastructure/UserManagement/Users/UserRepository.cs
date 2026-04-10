using Microsoft.EntityFrameworkCore;
using BLL = CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserRepository: BLL.IUserRepository
{
    private readonly UserManagementContext _context;

    public UserRepository(UserManagementContext context)
    {
        _context = context;
    }
    
    public async Task<BLL.User?> GetUserAsync(BLL.UserQuery query
                                                              , CancellationToken token = default)
    {
        var users = await GetUsersAsync(query, token);
        return users.SingleOrDefault();
    }

    public async Task<List<BLL.User>> GetUsersAsync(BLL.UserQuery query, CancellationToken token = default) // TODO надо дерево строить
    {
        var specification = new UserSpecificationBuilder().WithQuery(query).Build();
        var users =  GetAllUsersWithData().Where(specification).ToList();
        
        return users.Select(u =>
        {
            var bossName = u.Boss is null
                ? string.Empty
                : $"{u.Boss.LastName} {u.Boss.FirstName} {u.Boss.SecondName}";
            return new BLL.User(u.Id, u.FirstName, u.SecondName, u.LastName, u.BossId
                , u.Email, u.PositionId, u.Position.Name, u.DepartmentId, u.Department.Name, bossName);
        }).ToList();
    }

    public async Task AddUserAsync(BLL.User user, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        var newUser = new User(user.Email, user.FirstName, user.SecondName, user.LastName
                             , user.BossId, user.PositionId, user.DepartmentId);
        _context.Users.Add(newUser);
    }

    public async Task UpdateUserAsync(BLL.User user, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        var updatingUser = await _context.Users.SingleOrDefaultAsync(u => u.Id == user.Id, token);
        
        updatingUser.FirstName = user.FirstName;
        updatingUser.SecondName = user.SecondName;
        updatingUser.LastName = user.LastName;
        updatingUser.Email = user.Email;
        updatingUser.BossId = user.BossId;
        updatingUser.DepartmentId = user.DepartmentId;
        updatingUser.PositionId = user.PositionId;
    }

    // TODO Скорее всего, использоваться не будет? Или в случае отсутствия данных по оценке?
    public async Task RemoveUserAsync(int id, CancellationToken token = default)
    {
        var deletingUser = await _context.Users.SingleOrDefaultAsync(u => u.Id == id, token);
        ArgumentNullException.ThrowIfNull(deletingUser);
        _context.Remove(deletingUser);
    }

    public async Task SaveAllChangesAsync(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);

    private IQueryable<User> GetAllUsersWithData()
    {
        return _context.Users
            .Include(u => u.Department)
            .Include(u => u.Position)
            .Include(u => u.Boss)
            .Include(u => u.RoleLinks);
    }
}