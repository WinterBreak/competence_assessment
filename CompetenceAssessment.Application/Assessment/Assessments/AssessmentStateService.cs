using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Notifications;
using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Application.Assessment;

public class AssessmentStateService: IAssessmentStateService
{
    private readonly IEmailNotificationService _emailService;
    private readonly IUserService _userService;

    public AssessmentStateService(IEmailNotificationService emailService
    , IUserService userService)
    {
        _emailService = emailService;
        _userService = userService;
    }
    
    public void ProceedState(Domain.Assessment.Assessment assessment)
    {
        switch (assessment.State)
        {
            case AssessmentState.InProgress:
                InProgressProceed(assessment);
                break;
            case AssessmentState.Reviewing:
                ReviewingProceed(assessment);
                break;
        }
    }

    private void InProgressProceed(Domain.Assessment.Assessment assessment)
    {
        var needReview = assessment.Template.Weights.Any(w => w.Task.Type == TaskType.OpenQuestion);
        if (needReview)
        {
            assessment.State = AssessmentState.Reviewing;
            NotifyInspector(assessment, "AssessmentReviewing");
        }
        else
        {
            assessment.State = AssessmentState.Completed;
            NotifyCandidate(assessment, "AssessmentCompletion");
        }
    }

    private void ReviewingProceed(Domain.Assessment.Assessment assessment)
    {
        assessment.State = AssessmentState.Completed;
        assessment.EndDate = DateTime.UtcNow;
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