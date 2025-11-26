using System.Diagnostics;
using System.Runtime.InteropServices;
using Spectre.Console;

namespace rs.core.cli;

public class CliAuthenticationInteractionService : IAuthenticationInteractionService
{
    public string AskForSecret()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("Client Secret:")
            .Secret()
        );
    }
}

public class CLIHelper
{
    public static void OpenBrowser(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}
