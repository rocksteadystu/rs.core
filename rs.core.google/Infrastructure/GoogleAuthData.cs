using rs.core.DataAccess;

namespace rs.core.google.Infrastructure;

[DataName("google.auth")]
public class GoogleAuthData
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ClientSecret { get; set; }
}
