using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Fcg.UnitTests.Api;

public sealed class GamesEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _client;

    public GamesEndpointTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostJogos_WithAdministratorToken_ReturnsCreated()
    {
        var adminToken = await LoginAsAdministratorAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/jogos")
        {
            Content = JsonContent.Create(new
            {
                titulo = "Architecture Quest",
                descricao = "Educational game about software architecture.",
                preco = 79.90m,
                categoria = "Education"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<GameHttpResponse>(JsonOptions);

        Assert.NotNull(payload);
        Assert.Equal("Architecture Quest", payload.Titulo);
        Assert.True(payload.Ativo);
    }

    [Fact]
    public async Task PostJogos_WithRegularUserToken_ReturnsForbidden()
    {
        var userToken = await LoginAsRegularUserAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/jogos")
        {
            Content = JsonContent.Create(new
            {
                titulo = "Architecture Quest",
                descricao = "Educational game about software architecture.",
                preco = 79.90m,
                categoria = "Education"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetJogos_WithAuthenticatedUser_ReturnsCatalog()
    {
        var adminToken = await LoginAsAdministratorAsync();
        await CreateGameAsync(adminToken);
        var userToken = await LoginAsRegularUserAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/jogos");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<GameHttpResponse[]>(JsonOptions);

        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
        Assert.Contains(payload, game => game.Titulo == "Architecture Quest");
    }

    [Fact]
    public async Task PutJogos_WithAdministratorToken_ReturnsUpdatedGame()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var gameId = await CreateGameAsync(adminToken);

        using var request = new HttpRequestMessage(HttpMethod.Put, $"/jogos/{gameId}")
        {
            Content = JsonContent.Create(new
            {
                titulo = "Architecture Quest Reloaded",
                descricao = "Updated educational game about software architecture.",
                preco = 89.90m,
                categoria = "Strategy"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<GameHttpResponse>(JsonOptions);

        Assert.NotNull(payload);
        Assert.Equal("Architecture Quest Reloaded", payload.Titulo);
    }

    [Fact]
    public async Task DeleteJogos_WithAdministratorToken_HidesGameFromCatalog()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var gameId = await CreateGameAsync(adminToken);

        using var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/jogos/{gameId}");
        deleteRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var userToken = await LoginAsRegularUserAsync();
        using var getRequest = new HttpRequestMessage(HttpMethod.Get, "/jogos");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var getResponse = await _client.SendAsync(getRequest);
        var payload = await getResponse.Content.ReadFromJsonAsync<GameHttpResponse[]>(JsonOptions);

        Assert.NotNull(payload);
        Assert.DoesNotContain(payload, game => game.Id == gameId);
    }

    [Fact]
    public async Task PutJogos_WithRegularUserToken_ReturnsForbidden()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var gameId = await CreateGameAsync(adminToken);
        var userToken = await LoginAsRegularUserAsync();

        using var request = new HttpRequestMessage(HttpMethod.Put, $"/jogos/{gameId}")
        {
            Content = JsonContent.Create(new
            {
                titulo = "Architecture Quest Reloaded",
                descricao = "Updated educational game about software architecture.",
                preco = 89.90m,
                categoria = "Strategy"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<string> LoginAsAdministratorAsync()
    {
        var response = await _client.PostAsJsonAsync("/auth/login", new
        {
            email = "admin@fcg.local",
            senha = "Admin@123"
        });

        var payload = await response.Content.ReadFromJsonAsync<LoginHttpResponse>(JsonOptions);
        return payload!.Token;
    }

    private async Task<string> LoginAsRegularUserAsync()
    {
        var email = $"alice-{Guid.NewGuid():N}@example.com";

        await _client.PostAsJsonAsync("/usuarios", new
        {
            nome = $"Alice {Guid.NewGuid():N}",
            email,
            senha = "Secure@123"
        });

        var response = await _client.PostAsJsonAsync("/auth/login", new
        {
            email,
            senha = "Secure@123"
        });

        var payload = await response.Content.ReadFromJsonAsync<LoginHttpResponse>(JsonOptions);
        return payload!.Token;
    }

    private async Task<Guid> CreateGameAsync(string adminToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/jogos")
        {
            Content = JsonContent.Create(new
            {
                titulo = "Architecture Quest",
                descricao = "Educational game about software architecture.",
                preco = 79.90m,
                categoria = "Education"
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _client.SendAsync(request);
        var payload = await response.Content.ReadFromJsonAsync<GameHttpResponse>(JsonOptions);

        return payload!.Id;
    }

    private sealed class LoginHttpResponse
    {
        public string Token { get; init; } = string.Empty;
    }

    private sealed class GameHttpResponse
    {
        public Guid Id { get; init; }

        public string Titulo { get; init; } = string.Empty;

        public bool Ativo { get; init; }
    }
}
