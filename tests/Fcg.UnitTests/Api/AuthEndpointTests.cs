using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Fcg.UnitTests.Api;

public sealed class AuthEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _client;

    public AuthEndpointTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostLogin_WithValidCredentials_ReturnsJwt()
    {
        await RegisterDefaultUserAsync();

        var response = await _client.PostAsJsonAsync("/auth/login", new
        {
            email = "alice@example.com",
            senha = "Secure@123"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<LoginHttpResponse>(JsonOptions);

        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
        Assert.Equal("Bearer", payload.TipoToken);
        Assert.Equal("alice@example.com", payload.Email);
        Assert.Equal("User", payload.Perfil);
    }

    [Fact]
    public async Task PostLogin_WithInvalidPassword_ReturnsUnauthorized()
    {
        await RegisterDefaultUserAsync();

        var response = await _client.PostAsJsonAsync("/auth/login", new
        {
            email = "alice@example.com",
            senha = "Wrong@123"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_WithValidBearerToken_ReturnsCurrentUser()
    {
        await RegisterDefaultUserAsync();

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", new
        {
            email = "alice@example.com",
            senha = "Secure@123"
        });

        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginHttpResponse>(JsonOptions);

        Assert.NotNull(loginPayload);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/usuarios/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload.Token);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<CurrentUserHttpResponse>(JsonOptions);

        Assert.NotNull(payload);
        Assert.Equal("Alice Johnson", payload.Nome);
        Assert.Equal("alice@example.com", payload.Email);
        Assert.Equal("User", payload.Perfil);
    }

    private async Task RegisterDefaultUserAsync()
    {
        await _client.PostAsJsonAsync("/usuarios", new
        {
            nome = "Alice Johnson",
            email = "alice@example.com",
            senha = "Secure@123"
        });
    }

    private sealed class LoginHttpResponse
    {
        public string Token { get; init; } = string.Empty;

        public string TipoToken { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Perfil { get; init; } = string.Empty;
    }

    private sealed class CurrentUserHttpResponse
    {
        public string Nome { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Perfil { get; init; } = string.Empty;
    }
}
