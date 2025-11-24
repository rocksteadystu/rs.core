namespace rs.core.google.Infrastructure;

public interface IGoogleAuthenticator
{
    Task<TokenResponse> GetAccessTokens(GoogleAuthOptions options);
    Task<TokenResponse> RefreshTokens(string refreshToken, GoogleAuthOptions options);
}
