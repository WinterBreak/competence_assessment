using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentResultValidation: IValidationRule<Assessment>
{
    public async Task ValidateAsync(Assessment assessment, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        if (AssessmentType.Testing == assessment.Type)
        {
            return;
        }

        await Task.Run(() =>
        {
            if (assessment.Results.Any(r => r.Answer is null || r.Answer == string.Empty))
            {
                validationErrors.AddError(ErrorsConfg.EMPTY_FIELD, ErrorsConfg.EMPTY_FIELD_ERROR);
            }
        }, token);
    }
}