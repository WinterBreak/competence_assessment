using CompetenceAssessment.Domain.Assessment.DTO;

namespace CompetenceAssessment.Domain.Ports;

public interface IUserProvider
{
    Task<List<AssessmentParticipant>> GetAssessmentParticipantsAsync(IEnumerable<int> userIds
                                                                   , CancellationToken token = default);
}