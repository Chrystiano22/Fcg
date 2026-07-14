using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Fcg.UnitTests.Api;

public sealed class RegisterUserEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _client;

    public RegisterUserEndpointTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostUsuarios_WithValidPayload_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/usuarios", new
        {
            nome = "Alice Johnson",
            email = "alice@example.com",
            senha = "Secure@123"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Correlation-Id"));

        var payload = await response.Content.ReadFromJsonAsync<RegisterUserHttpResponse>(JsonOptions);

        Assert.NotNull(payload);
        Assert.NotEqual(Guid.Empty, payload.Id);
        Assert.Equal("Alice Johnson", payload.Nome);
        Assert.Equal("alice@example.com", payload.Email);
        Assert.Equal("User", payload.Perfil);
    }

    [Fact]
    public async Task PostUsuarios_WithDuplicateEmail_ReturnsBadRequest()
    {
        var request = new
        {
            nome = "Alice Johnson",
            email = "alice@example.com",
            senha = "Secure@123"
        };

        await _client.PostAsJsonAsync("/usuarios", request);
        var response = await _client.PostAsJsonAsync("/usuarios", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<HttpProblemDetails>(JsonOptions);

        Assert.NotNull(problemDetails);
        Assert.Equal("Validation error", problemDetails.Title);
        Assert.Equal("Email is already registered.", problemDetails.Detail);
        Assert.False(string.IsNullOrWhiteSpace(problemDetails.CorrelationId));
        Assert.True(response.Headers.Contains("X-Correlation-Id"));
    }

    private sealed class RegisterUserHttpResponse
    {
        public Guid Id { get; init; }

        public string Nome { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string Perfil { get; init; } = string.Empty;
    }

    private sealed class HttpProblemDetails
    {
        public string Title { get; init; } = string.Empty;

        public string Detail { get; init; } = string.Empty;

        public string CorrelationId { get; init; } = string.Empty;
    }
}
