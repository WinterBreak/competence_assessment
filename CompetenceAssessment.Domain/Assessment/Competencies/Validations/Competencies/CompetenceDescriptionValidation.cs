using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment.Competencies.Validations;

public class CompetenceDescriptionValidation: IValidationRule<Competence>
{
    private const int MAX_LENGTH = 500;
    
    public async Task ValidateAsync(Competence competence, ValidationErrors validationErrors
        , CancellationToken token = default)
    {
        await Task.Run(() =>
        {
            if (competence.Description?.Length > MAX_LENGTH)
            {
                validationErrors.AddError(ErrorsConfg.DESCRIPTION_ERROR, ErrorsConfg.MAX_LENGHT_ERROR);
                return;
            }
        }, token);
    }
}