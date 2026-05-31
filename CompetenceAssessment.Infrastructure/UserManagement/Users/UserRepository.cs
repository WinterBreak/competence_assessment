using CompetenceAssessment.Core.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BLL = CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserRepository: BLL.IUserRepository
{
    private readonly UserManagementContext _context;
    private readonly AssessmentContext _assessmentContext; // TODO плохо

    public UserRepository(UserManagementContext context
    , AssessmentContext assessmentContext)
    {
        _context = context;
        _assessmentContext = assessmentContext;
    }
    
    public async Task<BLL.User?> GetUserAsync(BLL.UserQuery query
                                                              , CancellationToken token = default)
    {
        var users = await GetUsersAsync(query, token);
        return users.SingleOrDefault();
    }

    public async Task<List<BLL.User>> GetUsersAsync(BLL.UserQuery query, CancellationToken token = default)
    {
        var specification = new UserSpecificationBuilder().WithQuery(query).Build();
        var users =  await GetAllUsersWithData()
            .Where(specification)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ThenBy(u => u.SecondName)
            .ToListAsync(token);
        
        return users.Select(u =>
        {
            var bossName = u.Boss is null
                ? string.Empty
                : $"{u.Boss.LastName} {u.Boss.FirstName} {u.Boss.SecondName}";
            var roles = u.RoleLinks.Select(rl => rl.Role.Name).ToList();
            return new BLL.User(u.Id, u.FirstName, u.SecondName, u.LastName, u.BossId
                , u.Email, u.PositionId, u.Position.Name, u.DepartmentId, u.Department.Name, bossName
                , roles);
        }).ToList();
    }

    public async Task<PaginatedResponse<BLL.User>> GetPaginatedUsersAsync(BLL.UserQuery query
        , CancellationToken token = default)
    {
        var specification = new UserSpecificationBuilder().WithQuery(query).Build();
        var users =  await GetAllUsersWithData()
            .Where(specification)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ThenBy(u => u.SecondName)
            .Skip(query.PageSize * (query.Page - 1))
            .Take(query.PageSize)
            .ToListAsync(token);
        
        var bllUsers = users.Select(u =>
        {
            var bossName = u.Boss is null
                ? string.Empty
                : $"{u.Boss.LastName} {u.Boss.FirstName} {u.Boss.SecondName}";
            var roles = u.RoleLinks.Select(rl => rl.Role.Name).ToList();
            return new BLL.User(u.Id, u.FirstName, u.SecondName, u.LastName, u.BossId
                , u.Email, u.PositionId, u.Position.Name, u.DepartmentId, u.Department.Name, bossName
                , roles);
        }).ToList();
        
        var totalCount = _context.Users.Where(specification).Count();
        return new PaginatedResponse<BLL.User>()
        {
            Items = bllUsers,
            CurrentPage = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task AddUserAsync(BLL.User user, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        var newUser = new User(user.Email, user.FirstName, user.SecondName, user.LastName
                             , user.BossId, user.PositionId, user.DepartmentId);
        var hasher = new PasswordHasher<User>();
        newUser.PasswordHash = hasher.HashPassword(newUser,"123456"); // TODO предполагается, что пароль корпоративный
        var roleId = _context.Roles.Single(r => r.Name == "Аттестуемый");
        newUser.RoleLinks.Add(new UserToRolesLink(newUser, roleId));
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
    
    public async Task RemoveUserAsync(int id, CancellationToken token = default)
    {
        var deletingUser = await _context.Users.SingleOrDefaultAsync(u => u.Id == id, token);
        ArgumentNullException.ThrowIfNull(deletingUser);
        
        var inspectors = _assessmentContext.AssessmentInspectors
            .Where(i => i.UserId == deletingUser.Id);
        _assessmentContext.AssessmentInspectors.RemoveRange(inspectors);
        var assessmentResults = _assessmentContext.AssessmentResults
            .Where(ar => ar.Assessment.UserId == deletingUser.Id);
        _assessmentContext.AssessmentResults.RemoveRange(assessmentResults);
        var assessments = _assessmentContext.Assessments.Where(a => a.UserId == deletingUser.Id);
        _assessmentContext.Assessments.RemoveRange(assessments);
        _context.UserRoleLinks.RemoveRange(deletingUser.RoleLinks);
        _context.Remove(deletingUser);
    }
    
    public async Task<bool> ValidatePasswordAsync(int userId, string password, CancellationToken token = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, token);
            
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            return false;
            
        return VerifyPassword(password, user.PasswordHash);
    }

    public async Task SaveAllChangesAsync(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);
    
    public async Task SaveChangesWithAsssessment(CancellationToken token = default)
    {
        await _assessmentContext.SaveChangesAsync(token);
        await _context.SaveChangesAsync(token);
    }
    
    private bool VerifyPassword(string password, string storedHash)
    {
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(null, storedHash, password);
        return result == PasswordVerificationResult.Success;
    }

    private IQueryable<User> GetAllUsersWithData()
    {
        return _context.Users
            .Include(u => u.Department)
            .Include(u => u.Position)
            .Include(u => u.Boss)
            .Include(u => u.RoleLinks)
            .ThenInclude(r => r.Role);
    }
}