using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentCandidateValidation: IValidationRule<Assessment>
{
    public async Task ValidateAsync(Assessment assessment, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(assessment);
        ArgumentNullException.ThrowIfNull(validationErrors);

        await Task.Run(() =>
        {
            if (assessment.Candidate.Id == null)
            {
                validationErrors.AddError(ErrorsConfg.EMPTY_FIELD, ErrorsConfg.EMPTY_FIELD_ERROR);
                return;
            }
        }, token);
    }
}