using System.Security.Claims;

using UserIdentity.Domain.Entities;

namespace UserIdentity.Application.Features.Authentication.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Gera um token de acesso para o usuário.
    /// </summary>
    /// <param name="user">O usuário para o qual o token será gerado.</param>
    /// <returns>O token de acesso gerado.</returns>
    string GenerateAccessToken(UserApplication user);

    /// <summary>
    /// Gera um token de atualização (refresh token) para o usuário.
    /// </summary>
    /// <param name="user">O usuário para o qual o token será gerado.</param>
    /// <returns>O token de atualização gerado.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Obtem claims do token de acesso.
    /// </summary>
    /// <param name="token">O token de acesso.</param>
    /// <returns>As claims extraídas do token.</returns>
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    /// <summary>
    /// Gera um par de tokens (token de acesso e token de atualização) para o usuário.
    /// </summary>
    /// <param name="user">O usuário para o qual os tokens serão gerados.</param>
    /// <returns>Uma tupla contendo o token de acesso e o token de atualização.</returns>
    (string token, string refreshToken) GenerateTokens(UserApplication user);

    public DateTime GetRefreshTokenExpirationTime(DateTime now);
    public DateTime GetAccessTokenExpirationTime(DateTime now);
}