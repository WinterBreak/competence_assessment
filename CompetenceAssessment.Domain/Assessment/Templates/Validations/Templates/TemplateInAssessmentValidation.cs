using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TemplateInAssessmentValidation(ITemplateValidationQueries queries): IValidationRule<ITemplate>
{
    public async Task ValidateAsync(ITemplate template, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        var isInTemplate = await queries.IsUsedInAssessmentAsync(template.Id, token);
        if (isInTemplate)
        {
            validationErrors.AddError(ErrorsConfg.ENTITY_USING_ERROR, ErrorsConfg.USING_NAME_ERROR);
        }
    }
}