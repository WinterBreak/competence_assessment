using System.ComponentModel.DataAnnotations;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITaskValidationService
{
    Task<ValidationErrors> ValidateCreatingTaskAsync(ITask task, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateUpdatingTaskAsync(ITask task, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateDeletingTaskAsync(int id, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateExistenceAsync(int id, CancellationToken token = default);
}