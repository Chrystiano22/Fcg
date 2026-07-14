using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Fcg.UnitTests.Api;

public sealed class PromotionsEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _client;

    public PromotionsEndpointTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostPromocoes_WithAdministratorToken_ReturnsCreated()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var gameId = await CreateGameAsync(adminToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/promocoes")
        {
            Content = JsonContent.Create(new
            {
                jogoId = gameId,
                nome = "Semana Tech",
                descricao = "Desconto especial para alunos.",
                percentualDesconto = 20m,
                inicioEm = DateTime.UtcNow.AddDays(-1),
                fimEm = DateTime.UtcNow.AddDays(5)
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PromotionHttpResponse>(JsonOptions);

        Assert.NotNull(payload);
        Assert.Equal(gameId, payload.JogoId);
        Assert.Equal("Semana Tech", payload.Nome);
        Assert.Equal(63.92m, payload.PrecoPromocional);
    }

    [Fact]
    public async Task PostPromocoes_WithRegularUserToken_ReturnsForbidden()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var gameId = await CreateGameAsync(adminToken);
        var userToken = await LoginAsRegularUserAsync();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/promocoes")
        {
            Content = JsonContent.Create(new
            {
                jogoId = gameId,
                nome = "Semana Tech",
                descricao = "Desconto especial para alunos.",
                percentualDesconto = 20m,
                inicioEm = DateTime.UtcNow.AddDays(-1),
                fimEm = DateTime.UtcNow.AddDays(5)
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetPromocoes_WithAuthenticatedUser_ReturnsPromotions()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var gameId = await CreateGameAsync(adminToken);
        await CreatePromotionAsync(adminToken, gameId);
        var userToken = await LoginAsRegularUserAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/promocoes");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PromotionHttpResponse[]>(JsonOptions);

        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
        Assert.Contains(payload, promotion => promotion.Nome == "Semana Tech");
    }

    private async Task CreatePromotionAsync(string adminToken, Guid gameId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/promocoes")
        {
            Content = JsonContent.Create(new
            {
                jogoId = gameId,
                nome = "Semana Tech",
                descricao = "Desconto especial para alunos.",
                percentualDesconto = 20m,
                inicioEm = DateTime.UtcNow.AddDays(-1),
                fimEm = DateTime.UtcNow.AddDays(5)
            })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        await _client.SendAsync(request);
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
    }

    private sealed class PromotionHttpResponse
    {
        public Guid JogoId { get; init; }

        public string Nome { get; init; } = string.Empty;

        public decimal PrecoPromocional { get; init; }
    }
}
