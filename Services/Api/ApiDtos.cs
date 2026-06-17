namespace TechMoveSystem.Services.Api;

public record LoginRequest(string ClientId, string ClientSecret);
public record TokenResponse(string AccessToken, DateTime ExpiresAt);
public record ContractStatusUpdateDto(string Status);
