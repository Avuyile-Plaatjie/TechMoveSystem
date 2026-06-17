using System.Net.Http.Headers;

namespace TechMoveSystem.Services.Api;

public class ApiAuthTokenHandler : DelegatingHandler
{
    private readonly IConfiguration _configuration;
    private string? _token;
    private DateTime _expiresAt;

    public ApiAuthTokenHandler(IConfiguration configuration) => _configuration = configuration;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_token is null || DateTime.UtcNow >= _expiresAt.AddMinutes(-5))
        {
            using var authClient = new HttpClient { BaseAddress = new Uri(_configuration["Api:BaseUrl"]!) };
            var login = new LoginRequest(_configuration["Api:ClientId"]!, _configuration["Api:ClientSecret"]!);
            var response = await authClient.PostAsJsonAsync("api/auth/token", login, cancellationToken);
            response.EnsureSuccessStatusCode();
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
            _token = tokenResponse!.AccessToken;
            _expiresAt = tokenResponse.ExpiresAt;
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        return await base.SendAsync(request, cancellationToken);
    }
}
