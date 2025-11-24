using System.ComponentModel.Design;
using rs.core.google.Infrastructure;
//using Microsoft.Extensions.DependencyInjection;

namespace rs.core.google;

public interface IGoogleTasksService
{
    
}

public class GoogleTasksService : IGoogleTasksService
{
    private readonly IGoogleServiceProvider _googleServiceProvider;

    public GoogleTasksService(IGoogleServiceProvider googleServiceProvider)
    {
        _googleServiceProvider = googleServiceProvider;
    }

    public async Task GetLists()
    {
        var serviceInitializer = await _googleServiceProvider.GetClientServiceInitializer();

        var taskService = new Google.Apis.Tasks.v1.TasksService(serviceInitializer);

        var lists = await taskService.Tasklists.List().ExecuteAsync();

        
    }
}

// public static class StartupExtensions
// {
//     public static IServiceCollection AddGoogleServices(this IServiceCollection services)
//     {
//         services.AddTr
//     }
// }