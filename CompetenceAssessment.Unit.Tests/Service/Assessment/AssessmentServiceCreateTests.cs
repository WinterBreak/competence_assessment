using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Assessments;

public class AssessmentServiceCreateTests
{
    [Fact]
    public async Task CreateAssessment_Should_Save_When_Valid()
    {
        var repository = new Mock<IAssessmentRepository>();
        var validation = new Mock<IAssessmentValidationService>();

        validation
            .Setup(v => v.ValidateCreatingAssessmentAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationErrors());

        var service = new AssessmentService(repository.Object, validation.Object);

        var command = new CreateAssessmentCommand(
            templateId: 1,
            type: AssessmentType.Testing,
            candidateId: 10,
            inspectorsIds: new List<int> { 20, 30 }
        );

        var result = await service.CreateAssessmentAsync(command);

        Assert.False(result.HasErrors);

        repository.Verify(r => r.AddAssessmentAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveAllChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAssessment_Should_Not_Save_When_Validation_Fails()
    {
        var repository = new Mock<IAssessmentRepository>();
        var validation = new Mock<IAssessmentValidationService>();

        var errors = new ValidationErrors();
        errors.AddMainError("error");

        validation
            .Setup(v => v.ValidateCreatingAssessmentAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errors);

        var service = new AssessmentService(repository.Object, validation.Object);

        var command = new CreateAssessmentCommand(
            templateId: 1,
            type: AssessmentType.Testing,
            candidateId: 10,
            inspectorsIds: new List<int> { 20, 30 }
        );

        var result = await service.CreateAssessmentAsync(command);

        Assert.True(result.HasErrors);

        repository.Verify(r => r.AddAssessmentAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}