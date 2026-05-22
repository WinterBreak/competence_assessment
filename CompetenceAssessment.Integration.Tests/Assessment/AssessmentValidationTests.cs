using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CompetenceAssessment.IntegrationTests.Validation;

public class AssessmentValidationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;

    public AssessmentValidationTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Update_Assessment_With_Empty_Answer_Should_Fail()
    {
        var request = new
        {
            assessmentId = 1,

            answers = new Dictionary<int, string?> { { 1, "" } },

            scores = new Dictionary<int, int> { { 1, 5 } },

            comments = new Dictionary<int, string> { { 1, "ok" } },

            comment = "test update"
        };
        
        var response = await _client.PatchAsJsonAsync(
            "/api/assessments",
            request
        );
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}