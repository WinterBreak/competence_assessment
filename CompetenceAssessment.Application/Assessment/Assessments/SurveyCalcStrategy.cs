using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class SurveyCalcStrategy : IAssessmentCalcStrategy
{
    public AssessmentCalculation Calculate(Domain.Assessment.Assessment assessment)
    {
        var calculation = new AssessmentCalculation(assessment);

        var competenceWeightsDict = assessment.Template.Model.Competencies
            .ToDictionary(cw => cw.Competence.Id, cw => cw.Weight);

        var taskWeightsDict = assessment.Template.Weights
            .ToDictionary(tw => tw.Task.Id, tw => tw.Weight);

        var competenciesToTasksDict = assessment.Template.Weights
            .ToDictionary(tw => tw.Task.Id, tw => tw.CompetenceId);

        var scale = (int)assessment.Template.Scale;

        calculation.ReferenceTotal = GetReferenceTotal(
            competenceWeightsDict,
            taskWeightsDict,
            competenciesToTasksDict,
            scale);

        calculation.CompetenceReferences = GetReferenceByCompetencies(
            assessment.Template.Weights,
            competenceWeightsDict,
            taskWeightsDict,
            scale);

        calculation.ReceivedTotal = GetReceivedTotal(
            assessment.Results,
            competenceWeightsDict,
            taskWeightsDict,
            competenciesToTasksDict);

        calculation.CompetenciesReceived = GetReceivedByCompetencies(
            assessment.Results,
            assessment.Template.Weights,
            competenceWeightsDict,
            taskWeightsDict);

        calculation.TotalReceivedPercentage =
            GetPercentageDiff(calculation.ReferenceTotal, calculation.ReceivedTotal);

        calculation.CompetenciesReceivedPercentage =
            GetPercentageByCompetencies(
                calculation.CompetenceReferences,
                calculation.CompetenciesReceived);

        return calculation;
    }

    private decimal GetReferenceTotal(
        Dictionary<int, decimal> competenceWeightsDict,
        Dictionary<int, decimal> taskWeightsDict,
        Dictionary<int, int> competenciesToTasksDict,
        int scale)
    {
        var total = 0m;

        foreach (var taskId in competenciesToTasksDict.Keys)
        {
            var competenceId = competenciesToTasksDict[taskId];

            competenceWeightsDict.TryGetValue(competenceId, out var cw);
            taskWeightsDict.TryGetValue(taskId, out var tw);

            total += cw * tw * scale;
        }

        return total;
    }

    private Dictionary<int, decimal> GetReferenceByCompetencies(
        List<TemplateWeight> taskWeights,
        Dictionary<int, decimal> competenceWeightsDict,
        Dictionary<int, decimal> taskWeightsDict,
        int scale)
    {
        var result = new Dictionary<int, decimal>();

        foreach (var group in taskWeights.GroupBy(x => x.CompetenceId))
        {
            competenceWeightsDict.TryGetValue(group.Key, out var cw);

            var sum = 0m;

            foreach (var t in group)
            {
                taskWeightsDict.TryGetValue(t.Task.Id, out var tw);
                sum += cw * tw * scale;
            }

            result[group.Key] = sum;
        }

        return result;
    }

    private decimal GetReceivedTotal(
        List<AssessmentResult> results,
        Dictionary<int, decimal> competenceWeightsDict,
        Dictionary<int, decimal> taskWeightsDict,
        Dictionary<int, int> competenciesToTasksDict)
    {
        var total = 0m;

        foreach (var r in results)
        {
            competenciesToTasksDict.TryGetValue(r.Task.Id, out var compId);

            competenceWeightsDict.TryGetValue(compId, out var cw);
            taskWeightsDict.TryGetValue(r.Task.Id, out var tw);

            total += cw * tw * r.Score;
        }

        return total;
    }

    private Dictionary<int, decimal> GetReceivedByCompetencies(
        List<AssessmentResult> results,
        List<TemplateWeight> taskWeights,
        Dictionary<int, decimal> competenceWeightsDict,
        Dictionary<int, decimal> taskWeightsDict)
    {
        var scores = results.ToDictionary(x => x.Task.Id, x => x.Score);
        var result = new Dictionary<int, decimal>();

        foreach (var group in taskWeights.GroupBy(x => x.CompetenceId))
        {
            competenceWeightsDict.TryGetValue(group.Key, out var cw);

            var sum = 0m;

            foreach (var t in group)
            {
                taskWeightsDict.TryGetValue(t.Task.Id, out var tw);
                scores.TryGetValue(t.Task.Id, out var score);

                sum += cw * tw * score;
            }

            result[group.Key] = sum;
        }

        return result;
    }

    private Dictionary<int, decimal> GetPercentageByCompetencies(
        Dictionary<int, decimal> reference,
        Dictionary<int, decimal> received)
    {
        var result = new Dictionary<int, decimal>();

        foreach (var kv in reference)
        {
            received.TryGetValue(kv.Key, out var rec);
            result[kv.Key] = GetPercentageDiff(kv.Value, rec);
        }

        return result;
    }

    private static decimal GetPercentageDiff(decimal reference, decimal received)
    {
        if (reference == 0)
            return 0;

        return received * 100 / reference;
    }
}