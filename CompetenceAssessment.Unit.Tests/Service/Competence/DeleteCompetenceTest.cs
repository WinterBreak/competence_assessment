using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class DeleteCompetenceTest
{
    [Theory]
    [InlineData(1)]
    public async Task Validate_Deleting_Valid(int id)
    {
        var command = new DeleteCompetenceCommand(id);
        
        var service = CreateService(id, false);
        var errors = await service.DeleteCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task Validate_Deleting_NotValid(int id)
    {
        var command = new DeleteCompetenceCommand(id);
        
        var service = CreateService(id, true);
        var errors = await service.DeleteCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    private CompetenceService CreateService(int id, bool queryResult)
    {
        var repoMoq = new Mock<ICompetenceRepository>();
        repoMoq.Setup(m => m
                .GetCompetenceAsync(It.IsAny<CompetenceQuery>(), CancellationToken.None))
            .ReturnsAsync(new Competence(1, "Коммуникабельность", null));
        
        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsUsedInModelsAsync(id, CancellationToken.None))
            .ReturnsAsync(queryResult);
        
        var validationService = new CompetenceValidationService(queriesMoq.Object);
        
        return new CompetenceService(repoMoq.Object, validationService);
    }
    
}