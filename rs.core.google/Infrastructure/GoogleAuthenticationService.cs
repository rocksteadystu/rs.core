using rs.core.DataAccess;

namespace rs.core.google.Infrastructure;

public class GoogleAuthenticationService : IGoogleAuthenticationService
{
    private readonly IDataService _localDataService;
    private readonly IGoogleAuthenticator _googleAuthenticator;

    public GoogleAuthenticationService(IDataService localDataService, IGoogleAuthenticator googleAuthenticator)
    {
        _localDataService = localDataService;
        _googleAuthenticator = googleAuthenticator;
    }

    public async Task<string> GetAccessToken()
    {
        var googleAuthData = await _localDataService.Get<GoogleAuthData>();

        var options = new GoogleAuthOptions("244062460514-fik7u6tek7c784r5vm6hu949sd218nis.apps.googleusercontent.com", "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", "http://localhost:6006");

        if(googleAuthData.AccessToken is null)
        {
            var dateAtAuthentication = DateTime.UtcNow;
            var tokens = await _googleAuthenticator.GetAccessTokens(options);
            googleAuthData.AccessToken = tokens.AccessToken;
            googleAuthData.RefreshToken = tokens.RefreshToken;
            googleAuthData.ExpiresAt = dateAtAuthentication.AddSeconds(tokens.ExpiresIn);
            await _localDataService.Save(googleAuthData);
        } 
        else if(googleAuthData.ExpiresAt is null || googleAuthData.ExpiresAt < DateTime.UtcNow)
        {
            var dateAtAuthentication = DateTime.UtcNow;
            var tokens = await _googleAuthenticator.RefreshTokens(googleAuthData.RefreshToken, options);
            googleAuthData.AccessToken = tokens.AccessToken;
            googleAuthData.RefreshToken = tokens.RefreshToken;
            googleAuthData.ExpiresAt = dateAtAuthentication.AddSeconds(tokens.ExpiresIn);
            await _localDataService.Save(googleAuthData);
        }

        return googleAuthData.AccessToken;
    }
}
