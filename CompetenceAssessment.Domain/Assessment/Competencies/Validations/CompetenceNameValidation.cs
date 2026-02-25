using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment.Competencies.Validations;

public class CompetenceNameValidation: IValidationRule<Competence>
{
    private const string ERROR_NAME = "Name";
    private const int MAX_NAME_LENGTH = 255;

    private readonly ICompetenceRepository _repository;

    public CompetenceNameValidation(ICompetenceRepository repository)
    {
        _repository = repository;
    }
    
    public async Task ValidateAsync(Competence competence, ValidationErrors validationErrors
        , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(competence);
        ArgumentNullException.ThrowIfNull(validationErrors);
        
        var name = competence.Name?.Trim();
        
        if (string.IsNullOrEmpty(name))
        {
            validationErrors.AddError(ERROR_NAME, ErrorsConfg.EMPTY_NAME_ERROR);
            return;
        }

        if (name.Length > MAX_NAME_LENGTH)
        {
            validationErrors.AddError(ERROR_NAME, ErrorsConfg.MAX_LENGHT_ERROR);
            return;
        }
        
        await CheckIfNameIsTaken(name, validationErrors, cancellationToken);
    }

    private async Task CheckIfNameIsTaken(string name, ValidationErrors validationErrors
        , CancellationToken cancellationToken = default)
    {
        var query = new CompetenceQuery(name: name);
        var competencies = await _repository
            .GetCompetenciesAsync(query, cancellationToken);
        if (competencies.Any())
        {
            validationErrors.AddError(ERROR_NAME, ErrorsConfg.USING_NAME_ERROR);
        }
    }
}