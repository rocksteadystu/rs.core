using rs.core.DataAccess;

namespace rs.core.google.Infrastructure;

public class GoogleAuthenticationService : IGoogleAuthenticationService
{
    private readonly IDataService _localDataService;
    private readonly IGoogleAuthenticator _googleAuthenticator;
    private readonly IAuthenticationInteractionService _authenticationInteractionService;

    public GoogleAuthenticationService(IDataService localDataService, IGoogleAuthenticator googleAuthenticator, IAuthenticationInteractionService authenticationInteractionService)
    {
        _localDataService = localDataService;
        _googleAuthenticator = googleAuthenticator;
        _authenticationInteractionService = authenticationInteractionService;
    }

    public async Task<string> GetAccessToken()
    {
        var googleAuthData = await _localDataService.Get<GoogleAuthData>();


        if(string.IsNullOrWhiteSpace(googleAuthData.ClientSecret))
        {
            googleAuthData.ClientSecret = _authenticationInteractionService.AskForSecret();
            await _localDataService.Save(googleAuthData);
        }

        var options = new GoogleAuthOptions(
            "244062460514-fik7u6tek7c784r5vm6hu949sd218nis.apps.googleusercontent.com", 
            googleAuthData.ClientSecret,
            "http://localhost:6006"
        );

        Console.WriteLine(googleAuthData.ClientSecret);

        if(googleAuthData.AccessToken is null || googleAuthData.RefreshToken is null)
        {
            Console.WriteLine("Getting new token");
            var dateAtAuthentication = DateTime.UtcNow;
            var tokens = await _googleAuthenticator.GetAccessTokens(options);
            googleAuthData.AccessToken = tokens.AccessToken;
            googleAuthData.RefreshToken = tokens.RefreshToken;
            googleAuthData.ExpiresAt = dateAtAuthentication.AddSeconds(tokens.ExpiresIn);
            await _localDataService.Save(googleAuthData);
        } 
        else if(googleAuthData.ExpiresAt is null || googleAuthData.ExpiresAt < DateTime.UtcNow)
        {
            Console.WriteLine("Refreshing token");
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
