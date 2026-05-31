using CompetenceAssessment.Domain.UserManagement;
using Microsoft.Extensions.Logging;

public class UserSynchronizationService : IUserSynchronizationService
{
    private readonly IExternalUserProvider _externalUserProvider;
    private readonly IPositionRepository _positionRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserSynchronizationService> _logger;

    public UserSynchronizationService(
        IExternalUserProvider externalUserProvider,
        IPositionRepository positionRepository,
        IDepartmentRepository departmentRepository,
        IUserRepository userRepository,
        ILogger<UserSynchronizationService> logger)
    {
        _externalUserProvider = externalUserProvider;
        _positionRepository = positionRepository;
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<SynchronizationResult> SynchronizeAsync(CancellationToken token = default)
    {
        var result = new SynchronizationResult();
        
        try
        {
            _logger.LogInformation("Нвчата синхронизация пользователей...");
            
            var externalUsers = await _externalUserProvider.GetUsersAsync(token);
            _logger.LogInformation("Retrieved {Count} users from external source", externalUsers.Count);
            
            var existingUsers = await _userRepository.GetUsersAsync(new UserQuery(), token);
            var existingPositions = await _positionRepository.GetAllAsync(token);
            var existingDepartments = await _departmentRepository.GetAllAsync(token);
            
            await EnsureUserPlaceInCompanyAsync(externalUsers, existingPositions, existingDepartments, result, token);
            existingPositions = await _positionRepository.GetAllAsync(token);
            existingDepartments = await _departmentRepository.GetAllAsync(token);
            
            var processedEmails = new HashSet<string>();
            await UpdateUsersAsync(externalUsers, existingUsers, existingPositions, existingDepartments
                                 , result,processedEmails, token);
            
            var usersToDelete = existingUsers.Where(u => !processedEmails.Contains(u.Email)).ToList();
            result.Deleted = usersToDelete.Count;
            
            foreach (var userToDelete in usersToDelete)
            {
                _logger.LogWarning("User {Email} (Id: {Id}) would be deleted, but deletion is stubbed", 
                    userToDelete.Email, userToDelete.Id);
                await _userRepository.RemoveUserAsync(userToDelete.Id, token);
            }
            
            await _userRepository.SaveChangesWithAsssessment(token);
            _logger.LogInformation("Синхронизация завершена. Добавлено: {Added}, Обновлено: {Updated}, Удалено: {Deleted}",
                result.Added, result.Updated, result.Deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при синхронизации пользователей");
            result.Errors.Add(ex.Message);
        }
        
        return result;
    }

    private async Task DeleteUsersAsync(List<User> existingUsers, HashSet<string> processedEmails
        , SynchronizationResult result)
    {
        var usersToDelete = existingUsers.Where(u => !processedEmails.Contains(u.Email)).ToList();
        result.Deleted = usersToDelete.Count;
            
        foreach (var userToDelete in usersToDelete)
        {
            // TODO: Реализовать удаление с учетом зависимостей (оценки, связи и т.д.)
            _logger.LogWarning("User {Email} (Id: {Id}) would be deleted, but deletion is stubbed", 
                userToDelete.Email, userToDelete.Id);
            // await _userRepository.RemoveUserAsync(userToDelete.Id, token);
        }
    }

    private async Task UpdateUsersAsync(List<ExternalUser> externalUsers
        , List<User> existingUsers, List<Position> existingPositions
        , List<Department> existingDepartments, SynchronizationResult result
        , HashSet<string> processedEmails, CancellationToken token)
    {
        
        foreach (var externalUser in externalUsers)
        {
            var existingUser = existingUsers.FirstOrDefault(u => 
                u.Email.Equals(externalUser.Email, StringComparison.OrdinalIgnoreCase));
                
            var positionId = existingPositions.First(p => p.Name == externalUser.PositionName).Id;
            var departmentId = existingDepartments.First(d => d.Name == externalUser.DepartmentName).Id;
            var bossId = await GetBossIdAsync(externalUser.BossEmail, existingUsers, externalUsers, token);
                
            if (existingUser == null)
            {
                await AddUserAsync(externalUser, positionId, departmentId, bossId, token);
                result.Added++;
            }
            else
            {
                await UpdateUserAsync(existingUser, externalUser, positionId, departmentId, bossId, token);
                result.Updated++;
            }
                
            processedEmails.Add(externalUser.Email);
        }
    }

    private async Task EnsureUserPlaceInCompanyAsync(List<ExternalUser> externalUsers
        , List<Position> existingPositions, List<Department> existingDepartments
        , SynchronizationResult result, CancellationToken token)
    {
        foreach (var externalUser in externalUsers)
        {
            await EnsurePositionAsync(externalUser.PositionName, existingPositions, result, token);
            await EnsureDepartmentAsync(externalUser.DepartmentName, existingDepartments, result, token);
        }  
        await _userRepository.SaveAllChangesAsync(token);
    }
    
    private async Task EnsurePositionAsync(
        string positionName, 
        List<Position> existingPositions,
        SynchronizationResult result,
        CancellationToken token)
    {
        if (!existingPositions.Any(p => p.Name == positionName))
        {
            var newPosition = new Position(positionName);
            await _positionRepository.AddAsync(newPosition, token);
            existingPositions.Add(newPosition);
            result.PositionCreated++;
        }
    }
    
    private async Task EnsureDepartmentAsync(
        string departmentName,
        List<Department> existingDepartments,
        SynchronizationResult result,
        CancellationToken token)
    {
        if (!existingDepartments.Any(d => d.Name == departmentName))
        {
            var newDepartment = new Department(departmentName);
            await _departmentRepository.AddAsync(newDepartment, token);
            existingDepartments.Add(newDepartment);
            result.DepartmentCreated++;
        }
    }
    
    private async Task<int?> GetBossIdAsync(
        string? bossEmail,
        List<User> existingUsers,
        List<ExternalUser> externalUsers,
        CancellationToken token)
    {
        if (string.IsNullOrEmpty(bossEmail))
            return null;
            
        var boss = existingUsers.FirstOrDefault(u => u.Email == bossEmail);
        
        if (boss == null)
        {
            // Руководитель еще не синхронизирован, ищем во внешних пользователях
            var externalBoss = externalUsers.FirstOrDefault(u => u.Email == bossEmail);
            if (externalBoss != null)
            {
                _logger.LogWarning("Руководитель {BossEmail} не найден в БД и будет создан позже", bossEmail);
            }
        }
        
        return boss?.Id;
    }
    
    private async Task AddUserAsync(
        ExternalUser externalUser,
        int positionId,
        int departmentId,
        int? bossId,
        CancellationToken token)
    {
        var user = new User{
            Email = externalUser.Email,
            FirstName = externalUser.FirstName,
            SecondName = externalUser.SecondName,
            LastName = externalUser.LastName,
            BossId = bossId,
            PositionId = positionId,
            DepartmentId = departmentId
        };
        
        await _userRepository.AddUserAsync(user, token);
        _logger.LogDebug("Добавлен новый пользователь: {Email}", externalUser.Email);
    }
    
    private async Task UpdateUserAsync(
        User existingUser,
        ExternalUser externalUser,
        int positionId,
        int departmentId,
        int? bossId,
        CancellationToken token)
    {
        bool hasChanges = false;
        
        if (existingUser.FirstName != externalUser.FirstName)
        {
            existingUser.FirstName = externalUser.FirstName;
            hasChanges = true;
        }
        
        if (existingUser.SecondName != externalUser.SecondName)
        {
            existingUser.SecondName = externalUser.SecondName;
            hasChanges = true;
        }
        
        if (existingUser.LastName != externalUser.LastName)
        {
            existingUser.LastName = externalUser.LastName;
            hasChanges = true;
        }
        
        if (existingUser.PositionId != positionId)
        {
            existingUser.PositionId = positionId;
            hasChanges = true;
        }
        
        if (existingUser.DepartmentId != departmentId)
        {
            existingUser.DepartmentId = departmentId;
            hasChanges = true;
        }
        
        if (existingUser.BossId != bossId)
        {
            existingUser.BossId = bossId;
            hasChanges = true;
        }
        
        if (hasChanges)
        {
            await _userRepository.UpdateUserAsync(existingUser, token);
            _logger.LogDebug("Обновлен пользователь: {Email}", externalUser.Email);
        }
    }
}