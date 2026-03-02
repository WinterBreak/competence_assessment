using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceModelWeightValidation: IValidationRule<CompetenceModel>
{
    // TODO минимальное количество? диапазон значений весов?
    
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
        });
    }
}