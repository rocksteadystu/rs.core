namespace rs.core.DataAccess;

public interface IDataService
{
    Task<T> Get<T>() where T: new();
    Task Save<T>(T value);
}
