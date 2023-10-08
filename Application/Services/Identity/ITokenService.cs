using Common.Requests;
using Common.Responses;
using Common.Responses.Wrappers;

namespace Application.Services.Identity;

public interface ITokenService
{
    Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest);
    Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
}