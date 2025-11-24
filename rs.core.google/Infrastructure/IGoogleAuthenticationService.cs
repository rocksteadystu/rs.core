namespace rs.core.google.Infrastructure;

public interface IGoogleAuthenticationService
{
     Task<string> GetAccessToken();
}
