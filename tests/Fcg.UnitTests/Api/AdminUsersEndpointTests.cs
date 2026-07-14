using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Fcg.UnitTests.Api;

public sealed class AdminUsersEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _client;

    public AdminUsersEndpointTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUsuarios_WithAdministratorToken_ReturnsUsers()
    {
        var adminToken = await LoginAsAdministratorAsync();
        await RegisterUserAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/usuarios");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<AdminUserHttpResponse[]>(JsonOptions);

        Assert.NotNull(payload);
        Assert.NotEmpty(payload);
        Assert.Contains(payload, user => user.Email == "admin@fcg.local");
    }

    [Fact]
    public async Task GetUsuarios_WithRegularUserToken_ReturnsForbidden()
    {
        var (_, userToken) = await RegisterAndLoginUserAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/usuarios");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatchUsuariosPerfil_WithAdministratorToken_PromotesUser()
    {
        var adminToken = await LoginAsAdministratorAsync();
        var (userId, email) = await RegisterUserAsync();

        using var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"/usuarios/{userId}/perfil")
        {
            Content = JsonContent.Create(new
            {
                perfil = "Administrator"
            })
        };
        patchRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var patchResponse = await _client.SendAsync(patchRequest);

        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", new
        {
            email,
            senha = "Secure@123"
        });
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<LoginHttpResponse>(JsonOptions);

        Assert.NotNull(loginPayload);
        Assert.Equal("Administrator", loginPayload.Perfil);
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

    private async Task<(Guid userId, string email)> RegisterUserAsync()
    {
        var email = $"admin-user-{Guid.NewGuid():N}@example.com";

        await _client.PostAsJsonAsync("/usuarios", new
        {
            nome = "Alice Johnson",
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

        return (mePayload!.Id, email);
    }

    private async Task<(Guid userId, string token)> RegisterAndLoginUserAsync()
    {
        var email = $"regular-{Guid.NewGuid():N}@example.com";

        await _client.PostAsJsonAsync("/usuarios", new
        {
            nome = "Regular User",
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

    private sealed class LoginHttpResponse
    {
        public string Token { get; init; } = string.Empty;

        public string Perfil { get; init; } = string.Empty;
    }

    private sealed class CurrentUserHttpResponse
    {
        public Guid Id { get; init; }
    }

    private sealed class AdminUserHttpResponse
    {
        public string Email { get; init; } = string.Empty;
    }
}
