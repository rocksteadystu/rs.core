using Google.Apis.Drive.v3;
using rs.core.google.Infrastructure;
using System.Management;

namespace rs.core.google;


public class GoogleDriveService : IGoogleDriveService
{
    private readonly IGoogleServiceProvider _googleServiceProvider;

    public GoogleDriveService(IGoogleServiceProvider googleServiceProvider)
    {
        _googleServiceProvider = googleServiceProvider;
    }

    public async Task<GoogleDriveFileInfo?> GetFolder(params string[] path)
    {
        var driveService = await GetService();

        var parent = "root";
        foreach(var step in path)
        {
            var request = driveService.Files.List();
            request.Q = $"name = '{step}' and mimeType = 'application/vnd.google-apps.folder' and '{parent}' in parents";
            var result = await request.ExecuteAsync();

            if (!result.Files.Any()) return null;

            parent = result.Files.Single().Id;
        }

        var file = await driveService.Files.Get(parent).ExecuteAsync();
        return new GoogleDriveFileInfo(file.Id, file.Name);
    }

    public async Task<IEnumerable<GoogleDriveFileInfo>> GetFilesInFolder(GoogleDriveFileInfo folderInfo)
    {
        var driveService = await GetService();

        //var folder = await GetFolder("Media", "Music");

        var listRequest = driveService.Files.List();
        //listRequest.Q = "name = 'Music'";
        listRequest.Q = $"'{folderInfo.Id}' in parents";

        var files = await listRequest.ExecuteAsync();

        return files.Files.Select(item => new GoogleDriveFileInfo(item.Id, item.Name));
    }

    private async Task<DriveService> GetService()
    {
        var serviceInitializer = await _googleServiceProvider.GetClientServiceInitializer();
        return new DriveService(serviceInitializer);
    }
}

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