using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class _360DegreeCalcStrategy : IAssessmentCalcStrategy
{
    public AssessmentCalculation Calculate(Domain.Assessment.Assessment assessment)
    {
        var aggregatedResults = AggregateScoresByTask(assessment.Results);
        var assessmentWithAggregatedResults = new Domain.Assessment.Assessment
        {
            Id = assessment.Id,
            Candidate = assessment.Candidate,
            StartDate = assessment.StartDate,
            EndDate = assessment.EndDate,
            Type = assessment.Type,
            Template = assessment.Template,
            Inspectors = assessment.Inspectors,
            Results = aggregatedResults,
            State = assessment.State,
            Comment = assessment.Comment
        };
        
        var surveyStrategy = new SurveyCalcStrategy();
        return surveyStrategy.Calculate(assessmentWithAggregatedResults);
    }

    private List<AssessmentResult> AggregateScoresByTask(List<AssessmentResult> results)
    {
        if (results == null || !results.Any())
            return new List<AssessmentResult>();
        
        SetScores(results);
        
        var groupedByTask = results
            .GroupBy(r => r.Task.Id)
            .Select(g => new
            {
                TaskId = g.Key,
                Task = g.First().Task,
                AverageScore = (int)Math.Round(g.Average(r => r.Score), MidpointRounding.AwayFromZero)
            })
            .ToList();

        return groupedByTask.Select(item => new AssessmentResult
        {
            Id = 0,
            AssessmentId = results.First().AssessmentId,
            Task = item.Task,
            Comment = string.Join("; ", results.Where(r => r.Task.Id == item.TaskId).Select(r => r.Comment)),
            Answer = null,
            Score = item.AverageScore
        }).ToList();
    }

    private void SetScores(List<AssessmentResult> results)
    {
        foreach (var res in results)
        {
            res.Score = int.TryParse(res.Answer, out var score) ? score : 0;
        }
    }
}