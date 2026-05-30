using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class AssessmentStateRepository: IAssessmentStateRepository
{
    private readonly AssessmentContext _context;

    public AssessmentStateRepository(AssessmentContext context)
    {
        _context = context;
    }
    
    public AssessmentState UpdateStateAsync(int assessmentId, CancellationToken token = default)
    {
        var assessment = _context.Assessments.SingleOrDefault(a => a.Id == assessmentId);
        ArgumentNullException.ThrowIfNull(assessment);
        
        switch (assessment.State)
        {
            case (int)AssessmentState.InProgress:
                InProgressProceed(assessment);
                break;
            case (int)AssessmentState.Reviewing:
                SetCompleted(assessment);
                break;
            case (int)AssessmentState.Waiting:
                SetCompleted(assessment);
                break;
        }
        
        return (AssessmentState)assessment.State;
    }

    public async System.Threading.Tasks.Task SaveAllChanges() => await _context.SaveChangesAsync();

    private void InProgressProceed(Assessment assessment)
    {
        if (assessment.AssessmentTypeId == (int)AssessmentType._360Degrees_)
        {
            ProceedFullDegreeState(assessment);
            return;
        }
        
        
        var needReview = assessment.Template.TemplateDetails
            .Any(w => w.Task.TaskTypeId == (int)TaskType.OpenQuestion);
        if (needReview)
        {
            assessment.State = (int)AssessmentState.Reviewing;
        }
        else
        {
            SetCompleted(assessment);
        }
    }

    private void ProceedFullDegreeState(Assessment assessment)
    {
        if (!assessment.ParentId.HasValue)
        {
            var hasUnfinishedChildren = _context.Assessments
                .Any(a => a.ParentId.HasValue
                                && a.ParentId == assessment.Id
                                && a.AssessmentTypeId != (int)AssessmentState.Completed);
            assessment.State = hasUnfinishedChildren ? (int)AssessmentState.Waiting : (int)AssessmentState.Completed;
            return;
        }
        
        SetCompleted(assessment);
        var hasUnfinished = _context.Assessments
            .Any(a => a.Id != assessment.Id
                      && a.ParentId.HasValue
                      && a.ParentId == assessment.ParentId
                      && a.AssessmentTypeId != (int)AssessmentState.Completed);
        if (!hasUnfinished)
        {
            var parent = _context.Assessments.Single(a => a.Id == assessment.ParentId);
            SetCompleted(parent);
        }
        SetCompleted(assessment);
    }

    private void SetCompleted(Assessment assessment)
    {
        assessment.State = (int)AssessmentState.Completed;
        assessment.EndDate = DateTime.UtcNow;
    }
}