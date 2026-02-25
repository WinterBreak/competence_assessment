using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment.Competencies.Validations;

public class CompetenceDescriptionValidation: IValidationRule<Competence>
{
    private const string ERROR_NAME = "Description";
    private const int MAX_LENGTH = 500;
    
    public async Task ValidateAsync(Competence competence, ValidationErrors validationErrors
        , CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            if (competence.Description?.Length > MAX_LENGTH)
            {
                validationErrors.AddError(ERROR_NAME, ErrorsConfg.MAX_LENGHT_ERROR);
                return;
            }
        }, cancellationToken);
    }
}