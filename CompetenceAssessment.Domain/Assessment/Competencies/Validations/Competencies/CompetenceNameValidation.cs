using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceNameValidation: IValidationRule<Competence>
{
    private const int MAX_NAME_LENGTH = 255;

    private readonly ICompetenceValidationQueries _validationQueries;

    public CompetenceNameValidation(ICompetenceValidationQueries validationQueries)
    {
        _validationQueries = validationQueries;
    }
    
    public async Task ValidateAsync(Competence competence, ValidationErrors validationErrors
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(competence);
        ArgumentNullException.ThrowIfNull(validationErrors);
        
        var name = competence.Name?.Trim();
        
        if (string.IsNullOrEmpty(name))
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
            return;
        }

        if (name.Length > MAX_NAME_LENGTH)
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.MAX_LENGHT_ERROR);
            return;
        }
        
        await CheckIfNameIsTaken(name, validationErrors, token);
    }

    private async Task CheckIfNameIsTaken(string name, ValidationErrors validationErrors
        , CancellationToken token = default)
    {
        var isNameTaken = await _validationQueries.IsNameTakenAsync(name, token);
        if (isNameTaken)
        {
            validationErrors.AddError(ErrorsConfg.USING_NAME_ERROR, ErrorsConfg.ENTITY_USING_ERROR);
        }
    }
}