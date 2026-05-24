using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Strategies;

public class TestCalcStrategyTests
{
    [Fact]
    public void Calculate_Should_Return_Stable_Reference_And_Received_Total()
    {
        var strategy = new TestCalcStrategy();

        var assessment = BuildAssessment();

        var result = strategy.Calculate(assessment);

        Assert.Equal(5m, result.ReferenceTotal);
        Assert.Equal(5m, result.ReceivedTotal);
        Assert.Equal(100m, result.TotalReceivedPercentage);
    }

    [Fact]
    public void Competence_Reference_Should_Be_Calculated_Correctly()
    {
        var strategy = new TestCalcStrategy();

        var result = strategy.Calculate(BuildAssessment());

        Assert.Single(result.CompetenceReferences);
        Assert.Equal(5m, result.CompetenceReferences[1]);
    }

    [Fact]
    public void Competence_Received_Should_Match_Reference_When_Correct()
    {
        var strategy = new TestCalcStrategy();

        var result = strategy.Calculate(BuildAssessment());

        Assert.Single(result.CompetenciesReceived);
        Assert.Equal(result.CompetenceReferences[1], result.CompetenciesReceived[1]);
    }

    [Fact]
    public void Competence_Percentage_Should_Be_100_When_All_Correct()
    {
        var strategy = new TestCalcStrategy();

        var result = strategy.Calculate(BuildAssessment());

        Assert.Single(result.CompetenciesReceivedPercentage);
        Assert.Equal(100m, result.CompetenciesReceivedPercentage[1]);
    }

    [Fact]
    public void Test_Score_Should_Be_Equal_To_Scale_When_Correct()
    {
        var strategy = new TestCalcStrategy();

        var assessment = BuildAssessment();

        var result = strategy.Calculate(assessment);

        var testResult = assessment.Results.First();

        Assert.Equal((int)ScaleType.FivePointScale, testResult.Score);
    }

    private Assessment BuildAssessment()
    {
        var task = new ITask
        {
            Id = 1,
            Type = TaskType.TestQuestion,
            Answers = new List<Answer>
            {
                new Answer(1, "A", true),
                new Answer(1, "B", false)
            }
        };

        var competence = new Competence
        {
            Id = 1,
            Name = "Competence"
        };

        var competenceWeight = new CompetenceWeight(
            competence,
            modelId: 1,
            weight: 1m
        );

        var model = new CompetenceModel
        {
            Id = 1,
            Name = "Model",
            Competencies = new List<CompetenceWeight>
            {
                competenceWeight
            }
        };

        var template = new ITemplate
        {
            Id = 1,
            Scale = ScaleType.FivePointScale,
            Model = model,
            Weights = new List<TemplateWeight>
            {
                new TemplateWeight(task, competence.Id, 1m)
            }
        };

        return new Assessment
        {
            Id = 1,
            Candidate = new AssessmentParticipant { Id = 10 },
            Template = template,
            Type = AssessmentType.Testing,
            Results = new List<AssessmentResult>
            {
                new AssessmentResult
                {
                    Task = task,
                    Answer = "A",
                    Score = 0
                }
            }
        };
    }
}