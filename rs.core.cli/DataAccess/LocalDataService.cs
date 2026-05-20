using System.Reflection;
using System.Text.Json;
using rs.core.DataAccess;

namespace rs.core.cli.DataAccess;

public class LocalDataService : ILocalDataService
{
    private string? _basePath = null;

    public async Task<T> Get<T>() where T: new()
    {
        var text = await File.ReadAllTextAsync(GetFilePath<T>());
        if(string.IsNullOrWhiteSpace(text)) text = "{}";
        return JsonSerializer.Deserialize<T>(text) ?? new T();
    }

    public async Task Save<T>(T value)
    {
        var text = JsonSerializer.Serialize(value);
        await File.WriteAllTextAsync(GetFilePath<T>(), text);
    }

    public async Task SetBasePath(string basePath)
    {
        var basePathFile = "./path.config";
        await File.WriteAllTextAsync(basePathFile, basePath);
    }

    private string GetFilePath<T>()
    {
        var fileName = typeof(T).GetCustomAttribute<DataNameAttribute>()?.Name ?? typeof(T).Name;
        var fullPath = Path.Combine(GetBasePath(), $"{fileName}.json");
        if(!File.Exists(fullPath))
        {
            var file = File.Create(fullPath);
            file.Close();
        }
        Console.WriteLine($"PATH: {fullPath}");
        return fullPath;
    }

    private string GetBasePath()
    {
        if (_basePath == null)
        {
            var basePathFile = "./path.config";
            if (File.Exists(basePathFile))
            {
                var contents = File.ReadAllLines(basePathFile);
                _basePath = contents.FirstOrDefault();
            }

            if (_basePath == null)
            {
                _basePath = "./";
            }
        }
        return _basePath;
    }

}