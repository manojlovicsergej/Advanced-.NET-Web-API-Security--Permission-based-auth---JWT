using Common.Requests;
using Common.Responses;

namespace Application.Services.Identity;

public interface ITokenService
{
    Task<TokenResponse> GetTokenAsync(TokenRequest token);
    Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}