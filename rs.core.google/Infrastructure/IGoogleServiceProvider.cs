using Google.Apis.Services;

namespace rs.core.google.Infrastructure;

public interface IGoogleServiceProvider
{
    Task<BaseClientService.Initializer> GetClientServiceInitializer();
}
