using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Validation;

public class SurveyTaskValidationServiceTests
{
    [Fact]
    public async Task ValidateCreatingTaskAsync_Should_Return_No_Errors_When_Task_Is_Valid()
    {
        var queries = CreateQueriesMock(isTaskExistByText: false);
        var service = new SurveyTaskValidationService(queries.Object);

        var task = CreateValidTask();

        var result = await service.ValidateCreatingTaskAsync(task);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTaskAsync_Should_Return_Error_When_Text_Is_Empty()
    {
        var queries = CreateQueriesMock();
        var service = new SurveyTaskValidationService(queries.Object);

        var task = CreateValidTask();
        task.Text = string.Empty;

        var result = await service.ValidateCreatingTaskAsync(task);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTaskAsync_Should_Return_Error_When_Task_Type_Is_None()
    {
        var queries = CreateQueriesMock();
        var service = new SurveyTaskValidationService(queries.Object);

        var task = CreateValidTask();
        task.Type = TaskType.None;

        var result = await service.ValidateCreatingTaskAsync(task);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingTaskAsync_Should_Return_Error_When_Task_Text_Already_Exists()
    {
        var queries = CreateQueriesMock(isTaskExistByText: true);
        var service = new SurveyTaskValidationService(queries.Object);

        var task = CreateValidTask();

        var result = await service.ValidateCreatingTaskAsync(task);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateUpdatingTaskAsync_Should_Return_No_Errors_When_Task_Is_Valid()
    {
        var queries = CreateQueriesMock(isTaskExistByText: false);
        var service = new SurveyTaskValidationService(queries.Object);

        var task = CreateValidTask(id: 1);

        var result = await service.ValidateUpdatingTaskAsync(task);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateDeletingTaskAsync_Should_Return_No_Errors_When_Task_Can_Be_Deleted()
    {
        var queries = CreateQueriesMock(
            isTaskExist: true,
            isUsedInTemplate: false);

        var service = new SurveyTaskValidationService(queries.Object);

        var result = await service.ValidateDeletingTaskAsync(1);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateDeletingTaskAsync_Should_Return_Error_When_Task_Does_Not_Exist()
    {
        var queries = CreateQueriesMock(isTaskExist: false);
        var service = new SurveyTaskValidationService(queries.Object);

        var result = await service.ValidateDeletingTaskAsync(1);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateDeletingTaskAsync_Should_Return_Error_When_Task_Is_Used_In_Template()
    {
        var queries = CreateQueriesMock(
            isTaskExist: true,
            isUsedInTemplate: true);

        var service = new SurveyTaskValidationService(queries.Object);

        var result = await service.ValidateDeletingTaskAsync(1);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateExistenceAsync_Should_Return_No_Errors_When_Task_Exists()
    {
        var queries = CreateQueriesMock(isTaskExist: true);
        var service = new SurveyTaskValidationService(queries.Object);

        var result = await service.ValidateExistenceAsync(1);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateExistenceAsync_Should_Return_Error_When_Task_Does_Not_Exist()
    {
        var queries = CreateQueriesMock(isTaskExist: false);
        var service = new SurveyTaskValidationService(queries.Object);

        var result = await service.ValidateExistenceAsync(1);

        Assert.True(result.HasErrors);
    }

    private static ITask CreateValidTask(int id = 0)
    {
        return new ITask(
            id,
            "How satisfied are you with teamwork?",
            TaskType.SurveyQuestion,
            []);
    }

    private static Mock<ITaskValidationQueries> CreateQueriesMock(
        bool isTaskExist = true,
        bool isTaskExistByText = false,
        bool isUsedInTemplate = false)
    {
        var mock = new Mock<ITaskValidationQueries>();

        mock.Setup(q =>
                q.IsTaskExistAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(isTaskExist);

        mock.Setup(q =>
                q.IsTaskExistAsync(
                    It.IsAny<string>(),
                    It.IsAny<TaskType>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(isTaskExistByText);

        mock.Setup(q =>
                q.IsUsedInTemplateAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(isUsedInTemplate);

        return mock;
    }
}