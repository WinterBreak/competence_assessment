using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class CompetenceValidationServiceTests
{
    private readonly Mock<ICompetenceValidationQueries> _queries;
    private readonly CompetenceValidationService _service;

    public CompetenceValidationServiceTests()
    {
        _queries = new Mock<ICompetenceValidationQueries>();
        _service = new CompetenceValidationService(_queries.Object);
    }

    [Fact]
    public async Task ValidateCreatingCompetenceAsync_Should_Return_No_Errors()
    {
        var competence = CreateCompetence();

        SetupNameTaken(false);

        var result = await _service.ValidateCreatingCompetenceAsync(competence);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateDeletingCompetenceAsync_Should_Return_Error_When_Used_In_Model()
    {
        SetupCompetenceExists(true);
        SetupUsedInModel(true);

        var result = await _service.ValidateDeletingCompetenceAsync(1);

        Assert.True(result.HasErrors);
    }

    private static Competence CreateCompetence(
        int id = 1,
        string name = "Коммуникабельность",
        string description = "Описание")
    {
        return new Competence(id, name, description);
    }

    private void SetupNameTaken(bool isTaken)
    {
        _queries.Setup(q => q.IsNameTakenAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(isTaken);
    }

    private void SetupCompetenceExists(bool exists)
    {
        _queries.Setup(q => q.IsCompetenceExistsAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists);
    }

    private void SetupUsedInModel(bool isUsed)
    {
        _queries.Setup(q => q.IsUsedInModelsAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(isUsed);
    }
}