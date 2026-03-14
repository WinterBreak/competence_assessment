using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Application.UserManagement;

public class UserService: IUserService
{
    private readonly IUserRepository _repository;
    private readonly IUserValidationService _validationService;

    public UserService(IUserRepository repository
        , IUserValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
    }
    
    public async Task<User?> GetUserAsync(UserQuery query, CancellationToken token = default)
        => await _repository.GetUserAsync(query, token);

    public async Task<List<User>> GetUsersAsync(UserQuery query, CancellationToken token = default)
        => await _repository.GetUsersAsync(query, token);

    public async Task<ValidationErrors> UpdateUserAsync(UpdateUserCommand command, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        var updatedUser = command.Update();
        
        var errors = await _validationService.ValidateUpdatingUserAsync(updatedUser, token);
        if (errors.HasErrors)
        {
            return errors;
        }

        await _repository.UpdateUserAsync(updatedUser, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }
}