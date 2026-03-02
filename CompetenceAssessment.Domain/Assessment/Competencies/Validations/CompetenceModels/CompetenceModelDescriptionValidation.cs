using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceModelDescriptionValidation: IValidationRule<CompetenceModel>
{
    private const string ERROR_NAME = "Description";
    private const int MAX_LENGTH = 1000;
    
    public async Task ValidateAsync(CompetenceModel competence, ValidationErrors validationErrors
        , CancellationToken token = default)
    {
        await Task.Run(() =>
        {
            if (competence.Description?.Length > MAX_LENGTH)
            {
                validationErrors.AddError(ERROR_NAME, ErrorsConfg.MAX_LENGHT_ERROR);
                return;
            }
        }, token);
    }
}