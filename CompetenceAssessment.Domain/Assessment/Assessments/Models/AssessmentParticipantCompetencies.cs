using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Domain.Assessment;

public class AssessmentParticipantCompetencies
{
    public User User { get; set; }
    
    public List<ParticipantCompetence> ParticipantCompetences { get; set; }
    
    public AssessmentParticipantCompetencies() {}

    public AssessmentParticipantCompetencies(User user, List<ParticipantCompetence> participantCompetences)
    {
        User = user;
        ParticipantCompetences = participantCompetences;
    }
}