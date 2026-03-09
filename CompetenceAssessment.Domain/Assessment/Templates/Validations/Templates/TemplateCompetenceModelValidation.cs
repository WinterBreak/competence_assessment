using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TemplateCompetenceModelValidation: IValidationRule<ITemplate>
{
    public async Task ValidateAsync(ITemplate template, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(validationErrors);

        if (template.CompetenceModelId == null)
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
        }
    }
}