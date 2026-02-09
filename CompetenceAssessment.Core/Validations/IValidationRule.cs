namespace CompetenceAssessment.Core.Validations;

public interface IValidationRule<in T> where T : class
{
    Task ValidateAsync(T entity,  ValidationErrors validationErrors
        , CancellationToken cancellationToken = default);
}