using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment.Competencies.Validations.Competencies;

public class CompetenceInModelValidation: IValidationRule<Competence>
{
    private const string ERROR_MESS = "Компетенция используется в моделях. Удаление запрещено.";
    
    private readonly ICompetenceValidationQueries _queries;

    public CompetenceInModelValidation(ICompetenceValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task ValidateAsync(Competence competence, ValidationErrors validationErrors
        , CancellationToken token = default)
    {
        var isUserInModels = await _queries.IsUsedInModelsAsync(competence.Id, token);
        if (isUserInModels)
        {
            validationErrors.AddError(ErrorsConfg.USING_NAME_ERROR, ERROR_MESS);
        }
    }
}