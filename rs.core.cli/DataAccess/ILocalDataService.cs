using rs.core.DataAccess;

namespace rs.core.cli.DataAccess;

public interface ILocalDataService : IDataService
{
    Task SetBasePath(string basePath);
}
