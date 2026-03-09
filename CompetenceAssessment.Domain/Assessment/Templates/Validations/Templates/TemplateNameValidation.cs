using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TemplateNameValidation: IValidationRule<ITemplate>
{
    private const int MAX_NAME_LENGTH = 255;
    
    private readonly ITemplateValidationQueries _queries;

    public TemplateNameValidation(ITemplateValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task ValidateAsync(ITemplate template, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(validationErrors);
        
        var name = template.Name?.Trim();
        
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
        
        var isNameTaken = await _queries.IsNameTakenAsync(name, token);
        if (isNameTaken)
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.USING_NAME_ERROR);
        }
    }
}