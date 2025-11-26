namespace rs.core.google;

public interface IGoogleTasksService
{
    Task<IEnumerable<GoogleTaskListInfo>> GetLists();
}
