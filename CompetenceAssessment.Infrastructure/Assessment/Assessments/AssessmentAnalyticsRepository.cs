using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Infrastructure.Assessment.Assessments;

public class AssessmentAnalyticsRepository: IAssessmentAnalyticsRepository
{
    private readonly IUserRepository _userRepository;
    private readonly ICompetenceRepository _competenceRepository;
    private readonly IAssessmentRepository _assessmentRepository;

    public AssessmentAnalyticsRepository(
          IUserRepository userRepository
        , ICompetenceRepository comppetenceRepository
        , IAssessmentRepository assessmentRepository)
    {
        _userRepository = userRepository;
        _competenceRepository = comppetenceRepository;
        _assessmentRepository = assessmentRepository;
    }
    
    public async Task<List<AssessmentParticipantCompetencies>> GetParticipantsCompetenciesAsync(
        CancellationToken token = default)
    {
        var users = await _userRepository.GetUsersAsync(new UserQuery(), token);
        var userIds = users.Select(u => u.Id).ToList();
        var assessments = await _assessmentRepository
            .GetAssessmentsAsync(new AssessmentQuery(candidateIds: userIds), token);
        var competencies = await _competenceRepository.GetCompetenciesAsync(new CompetenceQuery(), token);
    
        var competenceDict = competencies.ToDictionary(c => c.Id);
        var calculations = GetCalculations(assessments);
    
        var result = calculations
            .GroupBy(c => c.CandidateId)
            .Select(userGroup => 
            {
                var user = users.First(u => u.Id == userGroup.Key);
            
                var participantCompetences = userGroup
                    .SelectMany(calc => calc.CompetenciesReceived.Keys
                        .Select(compId => (CompId: compId, Calc: calc)))
                    .GroupBy(x => x.CompId)
                    .Select(group => group
                        .OrderByDescending(x => x.Calc.CompetenciesReceivedPercentage[x.CompId])
                        .First())
                    .Select(best => new ParticipantCompetence(
                        competence: competenceDict[best.CompId],
                        scale: (int)best.Calc.CompetenceReferences[best.CompId],
                        score: best.Calc.CompetenciesReceived[best.CompId],
                        percentage: best.Calc.CompetenciesReceivedPercentage[best.CompId]
                    ))
                    .ToList();
            
                return new AssessmentParticipantCompetencies(user, participantCompetences);
            })
            .ToList();
    
        return result;
    }

    private List<AssessmentCalculation> GetCalculations(IEnumerable<Domain.Assessment.Assessment> assessments)
    {
        var calcs = new List<AssessmentCalculation>();
        
        var testStrategy = new TestCalcStrategy();
        var surveyStrategy = new SurveyCalcStrategy();
        var fullDegreeStrategy = new _360DegreeCalcStrategy();
        
        foreach (var assessment in assessments)
        {
            switch (assessment.Type)
            {
                case AssessmentType.Testing:
                    calcs.Add(testStrategy.Calculate(assessment));
                    break;
                case AssessmentType.Survey:
                    calcs.Add(surveyStrategy.Calculate(assessment));
                    break;
                case AssessmentType._360Degrees_:
                    calcs.Add(fullDegreeStrategy.Calculate(assessment));
                    break;
            }
        }

        return calcs;
    }
}