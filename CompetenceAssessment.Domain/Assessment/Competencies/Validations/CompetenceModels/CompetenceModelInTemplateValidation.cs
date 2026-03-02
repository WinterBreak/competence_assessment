using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceModelInTemplateValidation: IValidationRule<CompetenceModel>
{
    private const string ERROR_MESS = "Модель используется в шаблонах. Удаление запрещено.";
    
    private readonly ICompetenceModelValidationQueries _queries;

    public CompetenceModelInTemplateValidation(ICompetenceModelValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task ValidateAsync(CompetenceModel model, ValidationErrors validationErrors,
        CancellationToken token = default)
    {
        var isInTemplate = await _queries.IsUsedInTemplateAsync(model.Id, token);
        if (isInTemplate)
        {
            validationErrors.AddError(ErrorsConfg.USING_NAME_ERROR, ERROR_MESS);
        }
    }
}