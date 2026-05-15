using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class TestCalcStrategy: IAssessmentCalcStrategy
{
public AssessmentCalculation Calculate(Domain.Assessment.Assessment assessment)
    {
        SetScores(assessment.Results, (int)assessment.Template.Scale);
        var calculation = new AssessmentCalculation(assessment);
        
        var competenceWeightsDict = assessment.Template.Model.Competencies
            .ToDictionary(cw => cw.Competence.Id, cw => cw.Weight);
        var taskWeightsDict = assessment.Template.Weights
            .ToDictionary(tw => tw.Task.Id, tw => tw.Weight);
        var competenciesToTasksDict = assessment.Template.Weights
            .ToDictionary(tw => tw.Task.Id, tw => tw.CompetenceId);
        
        calculation.ReferenceTotal = GetReferenceTotal(competenceWeightsDict, taskWeightsDict
            , competenciesToTasksDict, (int)assessment.Template.Scale);
        calculation.CompetenceReferences = GetReferenceByCompetencies(assessment.Template.Weights
            , competenceWeightsDict, taskWeightsDict, (int)assessment.Template.Scale);
        calculation.ReceivedTotal = GetReceivedTotal(assessment.Results, competenceWeightsDict
            , taskWeightsDict, competenciesToTasksDict);
        calculation.CompetenciesReceived = GetReceivedByCompetencies(assessment.Results
            , assessment.Template.Weights, competenceWeightsDict, taskWeightsDict);
        calculation.TotalReceivedPercentage = GetReferencePercentageDiff(calculation.ReferenceTotal
            , calculation.ReceivedTotal);
        calculation.CompetenciesReceivedPercentage = GetPercentageByCompetencies(
            calculation.CompetenceReferences, calculation.CompetenciesReceived);
        
        return calculation;
    }

    private decimal GetReferenceTotal(Dictionary<int, decimal> competenceWeightsDict
        , Dictionary<int, decimal> taskWeightsDict
        , Dictionary<int, int> competenciesToTasksDict
        , int scale)
    {
        var totalReference = decimal.Zero;
        foreach (var taskId in competenciesToTasksDict.Keys)
        {
            competenciesToTasksDict.TryGetValue(taskId, out var competenceId);
            competenceWeightsDict.TryGetValue(competenceId, out var competenceWeight);
            taskWeightsDict.TryGetValue(taskId, out var taskWeight);
            
            totalReference += competenceWeight * taskWeight * scale;
        }

        return totalReference;
    }

    private Dictionary<int, decimal> GetReferenceByCompetencies(
          List<TemplateWeight> taskWeights
        , Dictionary<int, decimal> competenceWeightsDict
        , Dictionary<int, decimal> taskWeightsDict
        , int scale)
    {
        var tasksByCompetencies = taskWeights
            .GroupBy(tw => tw.CompetenceId);

        var referenceByCompetencies = new Dictionary<int, decimal>();
        foreach (var group in tasksByCompetencies)
        {
            competenceWeightsDict.TryGetValue(group.Key, out var competenceWeight);
            var tasks = group.ToList();
            
            var reference = decimal.Zero;
            tasks.ForEach(t =>
            {
                taskWeightsDict.TryGetValue(t.Task.Id, out var taskWeight);
                reference += competenceWeight * taskWeight * scale;
            });
            
            referenceByCompetencies.Add(group.Key, reference);
        }
        
        return referenceByCompetencies;
    }
    
    private decimal GetReceivedTotal(
          List<AssessmentResult> results
        , Dictionary<int, decimal> competenceWeightsDict
        , Dictionary<int, decimal> taskWeightsDict
        , Dictionary<int, int> competenciesToTasksDict)
    {
        var totalReceived = decimal.Zero;
        
        foreach (var result in results)
        {
            competenciesToTasksDict.TryGetValue(result.Task.Id, out var competenceId);
            competenceWeightsDict.TryGetValue(competenceId, out var competenceWeight);
            taskWeightsDict.TryGetValue(result.Task.Id, out var taskWeight);
            
            totalReceived += competenceWeight * taskWeight * result.Score;
        }

        return totalReceived;
    }
    
    private Dictionary<int, decimal> GetReceivedByCompetencies(
          List<AssessmentResult> results
        , List<TemplateWeight> taskWeights
        , Dictionary<int, decimal> competenceWeightsDict
        , Dictionary<int, decimal> taskWeightsDict)
    {
        var tasksByCompetencies = taskWeights
            .GroupBy(tw => tw.CompetenceId);
        var scores = results
            .ToDictionary(s => s.Task.Id, s => s.Score);

        var referenceByCompetencies = new Dictionary<int, decimal>();
        foreach (var group in tasksByCompetencies)
        {
            competenceWeightsDict.TryGetValue(group.Key, out var competenceWeight);
            var tasks = group.ToList();
            
            var reference = decimal.Zero;
            tasks.ForEach(t =>
            {
                taskWeightsDict.TryGetValue(t.Task.Id, out var taskWeight);
                scores.TryGetValue(t.Task.Id, out var score);
                reference += competenceWeight * taskWeight * score;
            });
            
            referenceByCompetencies.Add(group.Key, reference);
        }
        
        return referenceByCompetencies;
    }

    private decimal GetReferencePercentageDiff(decimal reference, decimal received) 
        => GetPercentageDiff(reference, received);

    private Dictionary<int, decimal> GetPercentageByCompetencies(
        Dictionary<int, decimal> reference,
        Dictionary<int, decimal> received)
    {
        var percentages = new Dictionary<int, decimal>();

        foreach (var keyValuePair in reference)
        {
            received.TryGetValue(keyValuePair.Key, out var receivedValue);

            var percentage = GetPercentageDiff(keyValuePair.Value, receivedValue);
            percentages.Add(keyValuePair.Key, percentage);
        }

        return percentages;
    }

    private static void SetScores(IEnumerable<AssessmentResult> results, int scale)
    {
        foreach (var result in results)
        {
            if (result.Task.Type != TaskType.TestQuestion)
            {
                continue;
            }

            var correctAnswer = result.Task.Answers
                .FirstOrDefault(a => a.IsCorrect)?
                .Text;

            if (correctAnswer is null)
            {
                result.Score = 0;
                continue;
            }

            result.Score = result.Answer == correctAnswer ? scale : 0;
        }
    }
    
    private static decimal GetPercentageDiff(decimal reference, decimal received)
    {
        if (reference == 0)
            return 0;

        return received * 100 / reference;
    }
}