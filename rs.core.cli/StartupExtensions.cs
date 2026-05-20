using Microsoft.Extensions.DependencyInjection;
using rs.core.cli.DataAccess;
using rs.core.DataAccess;

namespace rs.core.cli;

public static class StartupExtensions
{
    public static IServiceCollection AddCoreCli(this IServiceCollection services)
    {
        return services.AddTransient<ILocalDataService, LocalDataService>()
        .AddTransient<IDataService, LocalDataService>()
        .AddTransient<IAuthenticationInteractionService, CliAuthenticationInteractionService>();
    }
}
