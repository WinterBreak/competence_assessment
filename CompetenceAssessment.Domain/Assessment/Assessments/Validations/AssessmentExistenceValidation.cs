using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentExistenceValidation: IValidationRule<Assessment>
{
    private readonly IAssessmentValidationQueries _queries;

    public AssessmentExistenceValidation(IAssessmentValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task ValidateAsync(Assessment assessment, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        var isExist = await _queries.IsExistAsync(assessment.Id, token);
        if (!isExist)
        {
            validationErrors.AddError(ErrorsConfg.NON_EXISTENT_ENTITY, ErrorsConfg.NON_EXISTENT_ENTITY_ERROR);
        }
    }
}