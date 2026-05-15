using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CompetenceAssessment.IntegrationTests.Auth;

public class AuthTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;

    public AuthTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Login_With_Invalid_Credentials_Returns_Failed()
    {
        var request = new
        {
            email = "wrong@mail.com",
            password = "123456"
        };
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_Without_User_Returns_Ok()
    {
        var response = await _client.PostAsync("/api/auth/logout", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}