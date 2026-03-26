using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentInspectorsValidation: IValidationRule<Assessment>
{
    public async Task ValidateAsync(Assessment assessment, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(assessment);
        ArgumentNullException.ThrowIfNull(validationErrors);

        await Task.Run(() =>
        {
            if (assessment.Inspectors is null || !assessment.Inspectors.Any())
            {
                validationErrors.AddError(ErrorsConfg.EMPTY_FIELD, ErrorsConfg.EMPTY_FIELD_ERROR);
                return;
            }

            if (assessment.Type == AssessmentType._360Degrees_
                && assessment.Inspectors.Count < 2)
            {
                validationErrors.AddError(ErrorsConfg.MULTIPLE_OPTIONS_REQUIRED
                    , ErrorsConfg.MULTIPLE_INSPECTOR_RQUIRED_ERRORS);
                return;
            }
        }, token);
    }
}