using System.Reflection;
using System.Text.Json;
using rs.core.DataAccess;

namespace rs.core.cli.DataAccess;

public class LocalDataService : IDataService
{

    private string _basePath = "./";

    public async Task<T> Get<T>() where T: new()
    {
        var text = await File.ReadAllTextAsync(GetFilePath<T>());
        return JsonSerializer.Deserialize<T>(text) ?? new T();
    }

    public async Task Save<T>(T value)
    {
        var text = JsonSerializer.Serialize(value);
        await File.WriteAllTextAsync(GetFilePath<T>(), text);
    }

    private string GetFilePath<T>()
    {
        var fileName = typeof(T).GetCustomAttribute<DataNameAttribute>()?.Name ?? typeof(T).Name;
        return Path.Combine(_basePath, $"{fileName}.json");
    }

}
