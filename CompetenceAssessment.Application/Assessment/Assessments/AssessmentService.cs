using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class AssessmentService: IAssessmentService
{
    private readonly IAssessmentRepository _repository;
    private readonly IAssessmentValidationService _validationService;

    public AssessmentService(IAssessmentRepository repository
        , IAssessmentValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
    }

    public async Task<Domain.Assessment.Assessment?> GetAssessmentAsync(int id
        , CancellationToken token = default)
    {
        var query = new AssessmentQuery(id: id);
        return await _repository.GetAssessmentAsync(query, token);
    }

    public async Task<List<Domain.Assessment.Assessment>> GetAssessmentsAsync(AssessmentQuery query
                                                                            , CancellationToken token = default)
        => await _repository.GetAssessmentsAsync(query, token);

    public async Task<ValidationErrors> CreateAssessmentAsync(CreateAssessmentCommand command
                                                            , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var newAssessment = command.Create();
        var errors = await _validationService.ValidateCreatingAssessmentAsync(newAssessment, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.AddAssessmentAsync(newAssessment, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }

    public async Task<ValidationErrors> UpdateAssessmentAsync(UpdateAssessmentCommand command
                                                            , CancellationToken token = default)
    {
        var errors = await _validationService.ValidateExistenceAsync(command.AssessmentId, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        var query = new AssessmentQuery(id: command.AssessmentId);
        var updatingAssessment = await _repository.GetAssessmentAsync(query, token);
        
        var updatedAssessment = command.Update(updatingAssessment);
        
        errors = await _validationService.ValidateUpdatingAssessmentAsync(updatedAssessment, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.UpdateAssessmentAsync(updatedAssessment, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }
}