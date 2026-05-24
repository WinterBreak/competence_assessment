using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class AssessmentStateService: IAssessmentStateService
{
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
        }
        else
        {
            assessment.State = AssessmentState.Completed;
        }
    }

    private void ReviewingProceed(Domain.Assessment.Assessment assessment)
    {
        assessment.State = AssessmentState.Completed;
        assessment.EndDate = DateTime.UtcNow;
    }
}