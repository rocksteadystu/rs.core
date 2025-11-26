using rs.core.google.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace rs.core.google;

public static class StartupExtensions
{
    public static IServiceCollection AddGoogleServices(this IServiceCollection services)
    {
        return services
            .AddTransient<IGoogleServiceProvider, GoogleServiceProvider>()
            .AddTransient<IGoogleAuthenticator, GoogleAuthenticator>()
            .AddTransient<IGoogleAuthenticationService, GoogleAuthenticationService>()
            .AddTransient<IGoogleTasksService, GoogleTasksService>();
    }
}