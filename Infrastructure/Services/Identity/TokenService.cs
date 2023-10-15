using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.AppConfigs;
using Application.Services.Identity;
using Common.Requests;
using Common.Requests.Identity;
using Common.Responses;
using Common.Responses.Wrappers;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Identity;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppConfiguration _appConfiguration;

    public TokenService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
        IOptions<AppConfiguration> appConfiguration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appConfiguration = appConfiguration.Value;
    }

    public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest)
    {
        // Validate user
        var user = await _userManager.FindByEmailAsync(tokenRequest.Email);
        // Check user
        if (user is null)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials.");
        }

        // Check if Active
        if (!user.IsActive)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("User not active.");
        }

        // Check email if email confirmed
        if (!user.EmailConfirmed)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Email is not confirmed.");
        }

        // Check password
        if (!await _userManager.CheckPasswordAsync(user, tokenRequest.Password))
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Credentials.");
        }

        // Generate refresh token
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);
        // Update user
        await _userManager.UpdateAsync(user);
        // Generate new token
        var token = await GenerateJWTAsync(user);
        // Return
        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
        };

        return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
    }

    public async Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        if (refreshTokenRequest is null)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Client Token.");
        }

        var userPrincipal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user is null)
            return await ResponseWrapper<TokenResponse>.FailAsync("User Not Found.");
        if (user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryDate <= DateTime.Now)
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid Client Token.");

        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
        };
        return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateJWTAsync(ApplicationUser user)
    {
        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        return token;
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes),
            signingCredentials: signingCredentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfiguration.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            var currentRole = await _roleManager.FindByNameAsync(role);
            var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
            permissionClaims.AddRange(allPermissionsForCurrentRole);
        }

        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

        return claims;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}