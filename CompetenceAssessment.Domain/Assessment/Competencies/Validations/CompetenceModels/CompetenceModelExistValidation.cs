using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceModelExistValidation: IValidationRule<CompetenceModel>
{
    private readonly ICompetenceModelValidationQueries _queries;

    public CompetenceModelExistValidation(ICompetenceModelValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task ValidateAsync(CompetenceModel model, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        var isCompetenceExist = await _queries.IsModelExist(model.Id, token);
        if (!isCompetenceExist)
        {
            validationErrors.AddError(ErrorsConfg.NON_EXISTENT_ENTITY, ErrorsConfg.NON_EXISTENT_ENTITY_ERROR);
        }
    }
}