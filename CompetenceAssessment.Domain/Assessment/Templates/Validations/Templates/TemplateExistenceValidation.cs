using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TemplateExistenceValidation: IValidationRule<ITemplate>
{
    private readonly ITemplateValidationQueries _queries;

    public TemplateExistenceValidation(ITemplateValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task ValidateAsync(ITemplate template, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        var isTemplateExist = await _queries.IsExistAsync(template.Id, token);
        if (!isTemplateExist)
        {
            validationErrors.AddError(ErrorsConfg.NON_EXISTENT_ENTITY, ErrorsConfg.NON_EXISTENT_ENTITY_ERROR);
        }
    }
}