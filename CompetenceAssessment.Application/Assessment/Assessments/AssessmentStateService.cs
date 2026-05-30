using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Notifications;
using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Application.Assessment;

public class AssessmentStateService: IAssessmentStateService
{
    private readonly IEmailNotificationService _emailService;
    private readonly IUserService _userService;
    private readonly IAssessmentStateRepository _repository;

    public AssessmentStateService(IEmailNotificationService emailService
    , IUserService userService
    , IAssessmentStateRepository repository)
    {
        _emailService = emailService;
        _userService = userService;
        _repository = repository;
    }
    
    public async Task ProceedStateAsync(Domain.Assessment.Assessment assessment)
    {
        var state = _repository.UpdateStateAsync(assessment.Id);
        switch (state)
        {
            case AssessmentState.Reviewing:
                NotifyInspector(assessment, "AssessmentReviewing");
                break;
            case AssessmentState.Completed:
                NotifyCandidate(assessment, "AssessmentCompletion");
                break;
        }

        await _repository.SaveAllChanges();
    }

    private async Task NotifyInspector(Domain.Assessment.Assessment assessment, string templateName)
    {
        var inspector = assessment.Inspectors.FirstOrDefault();
        if (inspector is null)
        {
            throw new ApplicationException("Эксперт не назначен!");
        }

        var user = await _userService.GetUserAsync(new UserQuery(id: inspector.Id));
        _emailService.SendTemplatedEmailAsync(templateName, new Dictionary<string, string>(), user.Email);
    }

    private async Task NotifyCandidate(Domain.Assessment.Assessment assessment, string templateName)
    {
        var user = await _userService.GetUserAsync(new UserQuery(id: assessment.Candidate.Id));
        _emailService.SendTemplatedEmailAsync(templateName, new Dictionary<string, string>(), user.Email);
    }
}