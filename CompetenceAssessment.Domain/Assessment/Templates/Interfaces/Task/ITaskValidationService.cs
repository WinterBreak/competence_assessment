using System.ComponentModel.DataAnnotations;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITaskValidationService
{
    Task<ValidationErrors> ValidateCreatingTask(ITask task, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateUpdatingTask(ITask task, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateDeletingTask(ITask task, CancellationToken token = default);
}