using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace rs.core.google.Infrastructure;

public class GoogleAuthenticator : IGoogleAuthenticator
{
    public async Task<TokenResponse> GetAccessTokens(GoogleAuthOptions options)
    {
        var authUrl = BuildAuthUrl(options);

        var code = GetOAuthCodeAsync(authUrl, options);
        Console.WriteLine($"Code is: {code}");

        var tokens = await GetAccessTokenAsync(code, options);

        Console.WriteLine($"AccessToken: {tokens.AccessToken}");
        Console.WriteLine($"RefreshToken: {tokens.RefreshToken}");
        Console.WriteLine($"ExpiresIn: {tokens.ExpiresIn}");

        return tokens;
    }

    public async Task<TokenResponse> RefreshTokens(string refreshToken, GoogleAuthOptions options)
    {
        return await RefreshTokenAsync(refreshToken, options);
    }

    static string GetOAuthCodeAsync(string authUrl, GoogleAuthOptions options)
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add(options.RedirectUri + "/");
        listener.Start();
        Console.WriteLine("Waiting for login response...");
        Process.Start(new ProcessStartInfo
        {
            FileName = authUrl,
            UseShellExecute = true
        });

        var listenerContext = listener.GetContext();
        var oauthCode = listenerContext.Request.QueryString["code"];

        try
        {
            if (string.IsNullOrEmpty(oauthCode))
            {
                throw new ApplicationException("OAuth code is missing or invalid");
            }

            const string responseHtml = @"
      <html>
        <body>
          <script>
            alert('Login successful! You can close this window now.');
          </script>
        </body>
      </html>";
            var buffer = Encoding.UTF8.GetBytes(responseHtml);
            listenerContext.Response.ContentLength64 = buffer.Length;
            listenerContext.Response.OutputStream.Write(buffer, 0, buffer.Length);
            listenerContext.Response.Close();

            return oauthCode;
        }
        finally
        {
            listener.Stop();
        }
    }


    static string BuildAuthUrl(GoogleAuthOptions options)
    {
        const string baseAuthUri = "https://accounts.google.com/o/oauth2/v2/auth";

        string[] scopes = [
            "https://www.googleapis.com/auth/userinfo.profile",
            "https://www.googleapis.com/auth/drive",
            "https://www.googleapis.com/auth/tasks",
        ];

        var authUriQueryParams = new Dictionary<string, string>
        {
            ["client_id"] = options.ClientId,
            ["redirect_uri"] = options.RedirectUri,
            ["response_type"] = "code",
            ["scope"] = string.Join(" ", scopes), // add the scopes you need
            ["access_type"] = "offline" // request a refresh token
        };

        var authUri = $"{baseAuthUri}?{string.Join("&", authUriQueryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}";
        Console.WriteLine($"Auth URL: {authUri}");
        return authUri;
    }

    static async Task<TokenResponse> GetAccessTokenAsync(string oauthCode, GoogleAuthOptions options)
    {
        const string tokenUri = "https://oauth2.googleapis.com/token";
        var tokenRequestParams = new Dictionary<string, string>
        {
            ["code"] = oauthCode,
            ["client_id"] = options.ClientId,
            ["client_secret"] = options.ClientSecret,
            ["redirect_uri"] = options.RedirectUri,
            ["grant_type"] = "authorization_code"
        };

        var tokenRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, tokenUri)
        {
            Content = new FormUrlEncodedContent(tokenRequestParams)
        };

        using var httpClient = new HttpClient();
        var tokenResponse = await httpClient.SendAsync(tokenRequest);
        var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();

        if (tokenResponse.IsSuccessStatusCode is false)
        {
            throw new ApplicationException($"Failed to get access token.");
        }

        var token = JsonSerializer.Deserialize<TokenResponse>(tokenResponseContent);

        if (token is null)
        {
            throw new ApplicationException($"Failed to parse access token response.");
        }

        return token;
    }

    static async Task<TokenResponse> RefreshTokenAsync(string refreshToken, GoogleAuthOptions options)
    {
        const string tokenUri = "https://oauth2.googleapis.com/token";
        var tokenRequestParams = new Dictionary<string, string>
        {
            ["refrsh_token"] = refreshToken,
            ["client_id"] = options.ClientId,
            ["client_secret"] = options.ClientSecret,
            //["redirect_uri"] = options.RedirectUri,
            ["grant_type"] = "refresh_token"
        };

        var tokenRequest = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, tokenUri)
        {
            Content = new FormUrlEncodedContent(tokenRequestParams)
        };

        using var httpClient = new HttpClient();
        var tokenResponse = await httpClient.SendAsync(tokenRequest);
        var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync();

        if (tokenResponse.IsSuccessStatusCode is false)
        {
            throw new ApplicationException($"Failed to get access token.");
        }

        var token = JsonSerializer.Deserialize<TokenResponse>(tokenResponseContent);

        if (token is null)
        {
            throw new ApplicationException($"Failed to parse access token response.");
        }

        return token;
    }

}
