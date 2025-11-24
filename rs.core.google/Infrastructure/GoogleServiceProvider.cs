using Google.Apis.Services;

namespace rs.core.google.Infrastructure;

public class GoogleServiceProvider : IGoogleServiceProvider
{
    private readonly IGoogleAuthenticationService _googleAuthenticationService;

    public GoogleServiceProvider(IGoogleAuthenticationService googleAuthenticationService)
    {
        _googleAuthenticationService = googleAuthenticationService;
    }

    public async Task<BaseClientService.Initializer> GetClientServiceInitializer()
    {
        var accessToken = await _googleAuthenticationService.GetAccessToken();

        var credentitals = Google.Apis.Auth.OAuth2.GoogleCredential.FromAccessToken(accessToken);

        return new BaseClientService.Initializer()
        {
            HttpClientInitializer = credentitals
        };
    }
}
