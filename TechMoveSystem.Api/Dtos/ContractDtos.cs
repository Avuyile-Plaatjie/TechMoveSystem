namespace TechMoveSystem.Api.Dtos;

public record ContractCreateDto(int ClientId, DateTime StartDate, DateTime EndDate, string Status, string ServiceLevel, string? SignedAgreementPath);
public record ContractStatusUpdateDto(string Status);
public record ClientCreateDto(string Name, string ContactDetails, string Region);
public record ServiceRequestCreateDto(string Description, decimal CostInUsd, int ContractId);
public record LoginRequest(string ClientId, string ClientSecret);
public record TokenResponse(string AccessToken, DateTime ExpiresAt);
