using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Fcg.UnitTests.Api;

public sealed class LibraryEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _client;

    public LibraryEndpointTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostUsuarioBiblioteca_WithAdministratorToken_ReturnsCreated()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var (userId, userToken) = await RegisterAndLoginUserAsync();
        var gameId = await CreateGameAsync(adminToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/usuarios/{userId}/biblioteca/{gameId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, "/biblioteca");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var libraryResponse = await _client.SendAsync(getRequest);
        var payload = await libraryResponse.Content.ReadFromJsonAsync<LibraryItemHttpResponse[]>(JsonOptions);

        Assert.NotNull(payload);
        Assert.Contains(payload, item => item.JogoId == gameId);
    }

    [Fact]
    public async Task PostUsuarioBiblioteca_WithRegularUserToken_ReturnsForbidden()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var (userId, userToken) = await RegisterAndLoginUserAsync();
        var gameId = await CreateGameAsync(adminToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/usuarios/{userId}/biblioteca/{gameId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostUsuarioBiblioteca_WithDuplicateGame_ReturnsBadRequest()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var (userId, _) = await RegisterAndLoginUserAsync();
        var gameId = await CreateGameAsync(adminToken);

        using var firstRequest = new HttpRequestMessage(HttpMethod.Post, $"/usuarios/{userId}/biblioteca/{gameId}");
        firstRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        await _client.SendAsync(firstRequest);

        using var secondRequest = new HttpRequestMessage(HttpMethod.Post, $"/usuarios/{userId}/biblioteca/{gameId}");
        secondRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _client.SendAsync(secondRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBiblioteca_WithAuthenticatedUser_ReturnsOwnedGames()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var (userId, userToken) = await RegisterAndLoginUserAsync();
        var gameId = await CreateGameAsync(adminToken);

        using var assignRequest = new HttpRequestMessage(HttpMethod.Post, $"/usuarios/{userId}/biblioteca/{gameId}");
        assignRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        await _client.SendAsync(assignRequest);

        using var getRequest = new HttpRequestMessage(HttpMethod.Get, "/biblioteca");
        getRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await _client.SendAsync(getRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<LibraryItemHttpResponse[]>(JsonOptions);

        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
        Assert.Contains(payload, item => item.Titulo == "Architecture Quest");
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

    private async Task<(Guid userId, string token)> RegisterAndLoginUserAsync()
    {
        var email = $"user-{Guid.NewGuid():N}@example.com";

        await _client.PostAsJsonAsync("/usuarios", new
        {
            nome = $"User {Guid.NewGuid():N}",
            email,
            senha = "Secure@123"
        });

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", new
        {
            email,
            senha = "Secure@123"
        });
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginHttpResponse>(JsonOptions);

        using var meRequest = new HttpRequestMessage(HttpMethod.Get, "/usuarios/me");
        meRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginPayload!.Token);

        var meResponse = await _client.SendAsync(meRequest);
        var mePayload = await meResponse.Content.ReadFromJsonAsync<CurrentUserHttpResponse>(JsonOptions);

        return (mePayload!.Id, loginPayload.Token);
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

    private sealed class CurrentUserHttpResponse
    {
        public Guid Id { get; init; }
    }

    private sealed class GameHttpResponse
    {
        public Guid Id { get; init; }
    }

    private sealed class LibraryItemHttpResponse
    {
        public Guid JogoId { get; init; }

        public string Titulo { get; init; } = string.Empty;
    }
}
