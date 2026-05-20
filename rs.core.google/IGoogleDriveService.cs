namespace rs.core.google
{
    public interface IGoogleDriveService
    {
        Task<GoogleDriveFolderInfo?> GetFolder(params string[] path);
        Task<IEnumerable<GoogleDriveFileInfo>> GetFilesInFolder(GoogleDriveFolderInfo folderInfo);
    }
}