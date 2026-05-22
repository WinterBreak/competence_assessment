using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Strategies;

public class SurveyCalcStrategyTests
{
    [Fact]
    public void Calculate_Should_Return_Valid_Totals()
    {
        var strategy = new SurveyCalcStrategy();

        var assessment = BuildAssessment();

        var result = strategy.Calculate(assessment);

        Assert.Equal(10m, result.ReferenceTotal);
        Assert.Equal(10m, result.ReceivedTotal);
        Assert.Equal(100m, result.TotalReceivedPercentage);
    }

    [Fact]
    public void CompetenceReferences_Should_Be_Calculated()
    {
        var strategy = new SurveyCalcStrategy();

        var result = strategy.Calculate(BuildAssessment());

        Assert.Single(result.CompetenceReferences);
        Assert.Equal(10m, result.CompetenceReferences[1]);
    }

    [Fact]
    public void CompetenciesReceived_Should_Match_Reference_When_Max_Score()
    {
        var strategy = new SurveyCalcStrategy();

        var result = strategy.Calculate(BuildAssessment());

        Assert.Equal(
            result.CompetenceReferences[1],
            result.CompetenciesReceived[1]
        );
    }

    [Fact]
    public void Competency_Percentage_Should_Be_100_When_Perfect_Result()
    {
        var strategy = new SurveyCalcStrategy();

        var result = strategy.Calculate(BuildAssessment());

        Assert.Equal(100m, result.CompetenciesReceivedPercentage[1]);
    }

    [Fact]
    public void Received_Should_Not_Exceed_Reference()
    {
        var strategy = new SurveyCalcStrategy();

        var result = strategy.Calculate(BuildAssessment());

        Assert.True(result.ReceivedTotal <= result.ReferenceTotal);
    }

    private Assessment BuildAssessment()
    {
        var task = new ITask
        {
            Id = 1,
            Type = TaskType.SurveyQuestion,
            Answers = new List<Answer>()
        };

        var competence = new Competence
        {
            Id = 1,
            Name = "Competence"
        };

        var model = new CompetenceModel
        {
            Id = 1,
            Name = "Model",
            Competencies = new List<CompetenceWeight>
            {
                new CompetenceWeight(competence, 1m)
            }
        };

        var template = new ITemplate
        {
            Id = 1,
            Scale = ScaleType.TenPointScale,
            Model = model,
            Type = TemplateType.Survey,
            Weights = new List<TemplateWeight>
            {
                new TemplateWeight(task, 1, 1m)
            }
        };

        return new Assessment
        {
            Id = 1,
            Candidate = new AssessmentParticipant { Id = 10 },
            Template = template,
            Type = AssessmentType.Survey,
            Results = new List<AssessmentResult>
            {
                new AssessmentResult
                {
                    Task = task,
                    Answer = "A",
                    Score = 10
                }
            }
        };
    }
}