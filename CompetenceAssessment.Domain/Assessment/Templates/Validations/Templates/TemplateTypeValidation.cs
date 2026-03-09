using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TemplateTypeValidation: IValidationRule<ITemplate>
{
    public async Task ValidateAsync(ITemplate template, ValidationErrors validationErrors
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(validationErrors);

        if (template.Type == TemplateType.None)
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
        }
    }
}