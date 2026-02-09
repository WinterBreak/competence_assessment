namespace CompetenceAssessment.Core.Validations;

public class ValidationErrors
{
    private readonly Dictionary<string, List<string>> _errors = new();
    
    public ValidationErrors() {}

    public void AddError(string errorName, string errorMessage)
    {
        if (!_errors.ContainsKey(errorName))
        {
            _errors.Add(errorName, []);
        }

        if (!_errors[errorName].Contains(errorMessage))
        {
            _errors[errorName].Add(errorMessage);
        }
    }

    public void AddMainError(string errorMessage)
    {
        AddError(errorName: string.Empty, errorMessage: errorMessage);
    }
    
    public Dictionary<string, List<string>> ErrorValues => _errors;
    
    public bool HasErrors => _errors.Any();
}