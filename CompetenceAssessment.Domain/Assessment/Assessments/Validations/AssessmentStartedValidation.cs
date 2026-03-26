using System.Data.Common;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentStartedValidation: IValidationRule<Assessment>
{
    private readonly IAssessmentValidationQueries _queries;

    public AssessmentStartedValidation(IAssessmentValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task ValidateAsync(Assessment assessment, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        var isStarted = await _queries.IsAssessmentStarted(assessment.Candidate.Id, assessment.Template.Id,
                                                           assessment.Type, token);
        if (isStarted)
        {
            validationErrors.AddError("AssessmentStarted", "Сотрудник уже проходит оценку");
        }
    }
}