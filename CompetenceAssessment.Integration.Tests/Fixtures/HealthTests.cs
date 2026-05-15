using CompetenceAssessment.IntegrationTests;
using Xunit;

public class HealthTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;

    public HealthTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Get_Should_Not_Fail()
    {
        var response = await _client.GetAsync("/api/assessments");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK ||
                    response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
    }
}