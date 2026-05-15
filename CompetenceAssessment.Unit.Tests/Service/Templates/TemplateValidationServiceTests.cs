using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Validation;

public class TemplateValidationServiceTests
{
    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_No_Errors_When_Template_Is_Valid()
    {
        var queries = CreateQueriesMock(isNameTaken: false);
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate();

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_Error_When_Name_Is_Empty()
    {
        var queries = CreateQueriesMock();
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate();
        template.Name = string.Empty;

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_Error_When_Name_Is_Too_Long()
    {
        var queries = CreateQueriesMock();
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate();
        template.Name = new string('a', 256);

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_Error_When_Name_Is_Taken()
    {
        var queries = CreateQueriesMock(isNameTaken: true);
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate();

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_Error_When_Template_Type_Is_None()
    {
        var queries = CreateQueriesMock();
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate();
        template.Type = TemplateType.None;

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_Error_When_Template_Has_No_Tasks()
    {
        var queries = CreateQueriesMock();
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate();
        template.Weights = [];

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_Error_When_Template_Has_Less_Than_Five_Tasks()
    {
        var queries = CreateQueriesMock();
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate(taskCount: 3);

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_Error_When_Survey_Template_Has_Wrong_Task_Type()
    {
        var queries = CreateQueriesMock();
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate(
            templateType: TemplateType.Survey,
            taskType: TaskType.TestQuestion);

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_Error_When_Test_Template_Has_Survey_Task()
    {
        var queries = CreateQueriesMock();
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate(
            templateType: TemplateType.Test,
            taskType: TaskType.SurveyQuestion);

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTemplateAsync_Should_Return_Error_When_Template_Has_Duplicate_Tasks()
    {
        var queries = CreateQueriesMock();
        var service = new TemplateValidationService(queries.Object);

        var task = CreateTask(1, TaskType.TestQuestion);

        var template = CreateValidTemplate();
        template.Weights =
        [
            new TemplateWeight(task, 1, 0.2m),
            new TemplateWeight(task, 1, 0.2m),
            new TemplateWeight(CreateTask(2, TaskType.TestQuestion), 1, 0.2m),
            new TemplateWeight(CreateTask(3, TaskType.TestQuestion), 1, 0.2m),
            new TemplateWeight(CreateTask(4, TaskType.TestQuestion), 1, 0.2m)
        ];

        var result = await service.ValidateCreatingTemplateAsync(template);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateUpdatingTemplateAsync_Should_Return_No_Errors_When_Template_Is_Valid()
    {
        var queries = CreateQueriesMock();
        var service = new TemplateValidationService(queries.Object);

        var template = CreateValidTemplate(id: 1);

        var result = await service.ValidateUpdatingTemplateAsync(template);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateDeletingTemplateAsync_Should_Return_No_Errors_When_Template_Can_Be_Deleted()
    {
        var queries = CreateQueriesMock(
            isTemplateExist: true,
            isUsedInAssessment: false);

        var service = new TemplateValidationService(queries.Object);

        var result = await service.ValidateDeletingTemplateAsync(1);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateDeletingTemplateAsync_Should_Return_Error_When_Template_Does_Not_Exist()
    {
        var queries = CreateQueriesMock(isTemplateExist: false);
        var service = new TemplateValidationService(queries.Object);

        var result = await service.ValidateDeletingTemplateAsync(1);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateDeletingTemplateAsync_Should_Return_Error_When_Template_Is_Used_In_Assessment()
    {
        var queries = CreateQueriesMock(
            isTemplateExist: true,
            isUsedInAssessment: true);

        var service = new TemplateValidationService(queries.Object);

        var result = await service.ValidateDeletingTemplateAsync(1);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateExistenceAsync_Should_Return_No_Errors_When_Template_Exists()
    {
        var queries = CreateQueriesMock(isTemplateExist: true);
        var service = new TemplateValidationService(queries.Object);

        var result = await service.ValidateExistenceAsync(1);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateExistenceAsync_Should_Return_Error_When_Template_Does_Not_Exist()
    {
        var queries = CreateQueriesMock(isTemplateExist: false);
        var service = new TemplateValidationService(queries.Object);

        var result = await service.ValidateExistenceAsync(1);

        Assert.True(result.HasErrors);
    }

    private static ITemplate CreateValidTemplate(
        int id = 0,
        int taskCount = 5,
        TemplateType templateType = TemplateType.Test,
        TaskType taskType = TaskType.TestQuestion)
    {
        var weights = new List<TemplateWeight>();

        for (var i = 1; i <= taskCount; i++)
        {
            weights.Add(new TemplateWeight(
                CreateTask(i, taskType),
                1,
                0.2m));
        }

        return new ITemplate(
            id,
            "Backend Template",
            templateType,
            ScaleType.FivePointScale,
            DateTime.UtcNow,
            new CompetenceModel
            {
                Id = 1,
                Competencies =
                [
                    new CompetenceWeight(
                        new Competence(1, "Communication", null),
                        1m)
                ]
            },
            weights);
    }

    private static ITask CreateTask(int id, TaskType type)
    {
        return new ITask(
            id,
            $"Task {id}",
            type,
            []);
    }

    private static Mock<ITemplateValidationQueries> CreateQueriesMock(
        bool isTemplateExist = true,
        bool isNameTaken = false,
        bool isUsedInAssessment = false)
    {
        var mock = new Mock<ITemplateValidationQueries>();

        mock.Setup(q =>
                q.IsExistAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(isTemplateExist);

        mock.Setup(q =>
                q.IsNameTakenAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(isNameTaken);

        mock.Setup(q =>
                q.IsUsedInAssessmentAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(isUsedInAssessment);

        return mock;
    }
}