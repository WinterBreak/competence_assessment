using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment.Competencies.Validations.Competencies;

public class CompetenceExistsValidation: IValidationRule<Competence>
{
    private readonly ICompetenceValidationQueries _queries;

    public CompetenceExistsValidation(ICompetenceValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task ValidateAsync(Competence competence, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        var isCompetenceExist = await _queries.IsCompetenceExistsAsync(competence.Id, token);
        if (!isCompetenceExist)
        {
            validationErrors.AddError(ErrorsConfg.NON_EXISTENT_ENTITY, ErrorsConfg.NON_EXISTENT_ENTITY_ERROR);
        }
    }
}