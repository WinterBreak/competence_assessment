using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Service;

public class DeleteCompetenceModelModelTest
{
    [Theory]
    [InlineData(1)]
    public async Task Validate_Deleting_Valid(int id)
    {
        var command = new DeleteCompetenceModelCommand(id);
        
        var service = CreateService(isUsed: false, isExist: true);
        var errors = await service.DeleteCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Theory]
    [InlineData(1)]
    public async Task Validate_Deleting_NotValid(int id)
    {
        var command = new DeleteCompetenceModelCommand(id);
        
        var service = CreateService(isUsed: false, isExist: false);
        var errors = await service.DeleteCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    private CompetenceModelService CreateService(bool isExist, bool isUsed)
    {
        var repoMoq = new Mock<ICompetenceModelRepository>();
        repoMoq.Setup(m => m
                .GetCompetenceModelAsync(It.IsAny<CompetenceModelQuery>(), CancellationToken.None))
            .ReturnsAsync(new CompetenceModel(1,"f ", null, DateTime.Now
                ,ValidWeights().ToList() ));
        
        var queriesMoq = new Mock<ICompetenceModelValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsModelExist(It.IsAny<int>()
                    , It.IsAny<CancellationToken>()))
            .ReturnsAsync(isExist);
        queriesMoq.Setup(q 
                => q.IsUsedInTemplateAsync(It.IsAny<int>()
                    , It.IsAny<CancellationToken>()))
            .ReturnsAsync(isUsed);
        
        var validationService = new CompetenceModelValidationService(queriesMoq.Object);
        
        return new CompetenceModelService(repoMoq.Object, validationService);
    }
    
    private static IEnumerable<CompetenceWeight> ValidWeights()
    {
        yield return new CompetenceWeight(new Competence(1, "f", ""), 1);
        yield return new CompetenceWeight(new Competence(2, "у", ""), 1);
        yield return new CompetenceWeight(new Competence(3, "d", ""), 1);
    }
}