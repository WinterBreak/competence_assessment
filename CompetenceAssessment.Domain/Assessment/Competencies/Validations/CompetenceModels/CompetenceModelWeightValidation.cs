using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceModelWeightValidation: IValidationRule<CompetenceModel>
{
    public const decimal MIN_WEIGHT = 0;
    public const decimal MAX_WEIGHT = 1;
    
    public async Task ValidateAsync(CompetenceModel model, ValidationErrors validationErrors,
        CancellationToken token = default)
    {
        await Task.Run(() =>
        {
            if (!model.Competencies.Any())
            {
                validationErrors.AddError(ErrorsConfg.DETAILS_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
            }
            
            var competenceIds = model.Competencies.Select(c => c.Competence.Id).ToList();
            var hasDublicates = competenceIds.Distinct().Count() != competenceIds.Count;
            if (hasDublicates)
            {
                validationErrors.AddError(ErrorsConfg.REPEATING_ERROR, ErrorsConfg.REPEATING_ENTITY_ERROR);
            }
            
            var hasWrongWeight = model.Competencies.Any(c => c.Weight < MIN_WEIGHT 
                                                                          || c.Weight > MAX_WEIGHT);
            if (hasWrongWeight)
            {
                validationErrors.AddError(ErrorsConfg.VALUE_RANGE_ERROR, ErrorsConfg.OUT_OF_RANGE_ERROR);
            }
        });
    }
}