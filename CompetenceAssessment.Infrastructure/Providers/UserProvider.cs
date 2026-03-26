using CompetenceAssessment.Domain.Assessment.DTO;
using CompetenceAssessment.Domain.Ports;
using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Infrastructure.Providers;

public class UserProvider: IUserProvider
{
    private readonly IUserRepository _userRepository;

    public UserProvider(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<List<AssessmentParticipant>> GetAssessmentParticipantsAsync(IEnumerable<int> userIds
                                                                                , CancellationToken token = default)
    {
        if (userIds == null || !userIds.Any())
        {
            return new List<AssessmentParticipant>();
        }
        
        var query = new UserQuery(ids: userIds.ToList());
        var users = await _userRepository.GetUsersAsync(query, token);
        return users.Select(u => new AssessmentParticipant(u.Id, u.BossId, u.FullName)).ToList();
    }
}