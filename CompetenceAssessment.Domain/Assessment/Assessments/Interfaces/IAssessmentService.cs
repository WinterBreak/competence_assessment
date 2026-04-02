using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentService
{
    Task<Assessment?> GetAssessmentAsync(AssessmentQuery query, CancellationToken token = default);
    
    Task<List<Assessment>> GetAssessmentsAsync(AssessmentQuery query, CancellationToken token = default);
    
    Task<ValidationErrors> CreateAssessmentAsync(CreateAssessmentCommand command
        , CancellationToken token = default);
    
    Task<ValidationErrors> UpdateAssessmentAsync(UpdateAssessmentCommand command
        , CancellationToken token = default);
 }