using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentService
{
    Task<Assessment?> GetAssessmentAsync(AssessmentQuery query, CancellationToken cancellationToken = default);
    
    Task<List<Assessment>> GetAssessmentsAsync(AssessmentQuery query, CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> CreateAssessmentAsync(CreateAssessmentCommand command
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> UpdateAssessmentAsync(UpdateAssessmentCommand command
        , CancellationToken cancellationToken = default);
 }