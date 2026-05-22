using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Validation;

public class CompetenceModelValidationServiceTests
{
    [Fact]
    public async Task ValidateCreatingCompetenceModelAsync_Should_Return_No_Errors_When_Model_Is_Valid()
    {
        var queries = CreateQueriesMock(isNameTaken: false);
        var service = new CompetenceModelValidationService(queries.Object);

        var model = CreateModel(
            name: "Backend Model",
            description: "Description",
            competencies:
            [
                new CompetenceWeight(
                    new Competence(1, "C#", null),
                    0.5m),

                new CompetenceWeight(
                    new Competence(2, "SQL", null),
                    0.5m)
            ]);

        var result = await service.ValidateCreatingCompetenceModelAsync(model);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingCompetenceModelAsync_Should_Return_Error_When_Name_Is_Empty()
    {
        var queries = CreateQueriesMock();
        var service = new CompetenceModelValidationService(queries.Object);

        var model = CreateModel(
            name: string.Empty,
            competencies: CreateValidCompetencies());

        var result = await service.ValidateCreatingCompetenceModelAsync(model);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingCompetenceModelAsync_Should_Return_Error_When_Name_Is_Too_Long()
    {
        var queries = CreateQueriesMock();
        var service = new CompetenceModelValidationService(queries.Object);

        var model = CreateModel(
            name: new string('a', 256),
            competencies: CreateValidCompetencies());

        var result = await service.ValidateCreatingCompetenceModelAsync(model);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingCompetenceModelAsync_Should_Return_Error_When_Name_Is_Taken()
    {
        var queries = CreateQueriesMock(isNameTaken: true);
        var service = new CompetenceModelValidationService(queries.Object);

        var model = CreateModel(
            name: "Backend Model",
            competencies: CreateValidCompetencies());

        var result = await service.ValidateCreatingCompetenceModelAsync(model);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingCompetenceModelAsync_Should_Return_Error_When_Description_Is_Too_Long()
    {
        var queries = CreateQueriesMock();
        var service = new CompetenceModelValidationService(queries.Object);

        var model = CreateModel(
            name: "Backend Model",
            description: new string('a', 1001),
            competencies: CreateValidCompetencies());

        var result = await service.ValidateCreatingCompetenceModelAsync(model);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingCompetenceModelAsync_Should_Return_Error_When_Competencies_Are_Empty()
    {
        var queries = CreateQueriesMock();
        var service = new CompetenceModelValidationService(queries.Object);

        var model = CreateModel(
            name: "Backend Model",
            competencies: []);

        var result = await service.ValidateCreatingCompetenceModelAsync(model);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingCompetenceModelAsync_Should_Return_Error_When_Weights_Are_Not_Equal_One()
    {
        var queries = CreateQueriesMock();
        var service = new CompetenceModelValidationService(queries.Object);

        var model = CreateModel(
            name: "Backend Model",
            competencies:
            [
                new CompetenceWeight(
                    new Competence(1, "C#", null),
                    1.3m),

                new CompetenceWeight(
                    new Competence(2, "SQL", null),
                    0.3m)
            ]);

        var result = await service.ValidateCreatingCompetenceModelAsync(model);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateUpdatingCompetenceModelAsync_Should_Return_No_Errors_When_Model_Is_Valid()
    {
        var queries = CreateQueriesMock(isNameTaken: false);
        var service = new CompetenceModelValidationService(queries.Object);

        var model = CreateModel(
            id: 1,
            name: "Updated Model",
            competencies: CreateValidCompetencies());

        var result = await service.ValidateUpdatingCompetenceModelAsync(model);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateDeletingCompetenceModelAsync_Should_Return_No_Errors_When_Model_Is_Not_Used()
    {
        var queries = CreateQueriesMock(isUsedInTemplate: false);
        var service = new CompetenceModelValidationService(queries.Object);

        var model = new CompetenceModel { Id = 1 };

        var result = await service.ValidateDeletingCompetenceModelAsync(model);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateDeletingCompetenceModelAsync_Should_Return_Error_When_Model_Is_Used_In_Template()
    {
        var queries = CreateQueriesMock(isUsedInTemplate: true);
        var service = new CompetenceModelValidationService(queries.Object);

        var model = new CompetenceModel { Id = 1 };

        var result = await service.ValidateDeletingCompetenceModelAsync(model);

        Assert.True(result.HasErrors);
    }

    [Fact]
    public async Task ValidateExistence_Should_Return_No_Errors_When_Model_Exists()
    {
        var queries = CreateQueriesMock(isExist: true);
        var service = new CompetenceModelValidationService(queries.Object);

        var result = await service.ValidateExistence(1);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateExistence_Should_Return_Error_When_Model_Does_Not_Exist()
    {
        var queries = CreateQueriesMock(isExist: false);
        var service = new CompetenceModelValidationService(queries.Object);

        var result = await service.ValidateExistence(1);

        Assert.True(result.HasErrors);
    }

    private static CompetenceModel CreateModel(
        int id = 0,
        string name = "Model",
        string? description = "Description",
        List<CompetenceWeight>? competencies = null)
    {
        return new CompetenceModel(
            id,
            name,
            description,
            DateTime.UtcNow,
            competencies ?? CreateValidCompetencies());
    }

    private static List<CompetenceWeight> CreateValidCompetencies()
    {
        return
        [
            new CompetenceWeight(
                new Competence(1, "Communication", null),
                0.5m),

            new CompetenceWeight(
                new Competence(2, "Leadership", null),
                0.5m)
        ];
    }

    private static Mock<ICompetenceModelValidationQueries> CreateQueriesMock(
        bool isNameTaken = false,
        bool isExist = true,
        bool isUsedInTemplate = false)
    {
        var mock = new Mock<ICompetenceModelValidationQueries>();

        mock.Setup(q =>
                q.IsNameTakenAsync(
                    It.IsAny<string>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(isNameTaken);

        mock.Setup(q =>
                q.IsModelExist(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(isExist);

        mock.Setup(q =>
                q.IsUsedInTemplateAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(isUsedInTemplate);

        return mock;
    }
}