using System.Net;
using System.Net.Http.Json;
using CompetenceAssessment.Domain.Assessment;
using Xunit;

namespace CompetenceAssessment.IntegrationTests.Assessment;

public class AssessmentFlowTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;

    public AssessmentFlowTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Create_Assessment_Should_Return_400_When_Template_Is_Missing()
    {
        var request = new
        {
            templateId = 0,
            candidateId = 1,
            type = (int)AssessmentType.Testing,
            inspectorsIds = new[] { 2 }
        };

        var response = await _client.PostAsJsonAsync("/api/assessments", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // [Fact]
    // public async Task Create_Assessment_Should_Succeed_With_Valid_Data()
    // {
    //     var request = new
    //     {
    //         templateId = 1,
    //         candidateId = 1,
    //         type = (int)AssessmentType.Testing,
    //         inspectorsIds = new[] { 2 }
    //     };
    //
    //     var response = await _client.PostAsJsonAsync("/api/assessments", request);
    //
    //     Assert.True(
    //         response.StatusCode == HttpStatusCode.OK ||
    //         response.StatusCode == HttpStatusCode.Created
    //     );
    // }
}