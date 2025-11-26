using rs.core.google.Infrastructure;

namespace rs.core.google;

public class GoogleTasksService : IGoogleTasksService
{
    private readonly IGoogleServiceProvider _googleServiceProvider;

    public GoogleTasksService(IGoogleServiceProvider googleServiceProvider)
    {
        _googleServiceProvider = googleServiceProvider;
    }

    public async Task<IEnumerable<GoogleTaskListInfo>> GetLists()
    {
        var serviceInitializer = await _googleServiceProvider.GetClientServiceInitializer();

        var taskService = new Google.Apis.Tasks.v1.TasksService(serviceInitializer);

        var lists = await taskService.Tasklists.List().ExecuteAsync();

        return lists.Items.Select(item => new GoogleTaskListInfo(item.Id, item.Title));
    }
}