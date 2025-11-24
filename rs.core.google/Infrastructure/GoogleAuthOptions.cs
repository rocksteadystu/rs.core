namespace rs.core.google.Infrastructure;

public record GoogleAuthOptions(
    string ClientId,
    string ClientSecret,
    string RedirectUri);
