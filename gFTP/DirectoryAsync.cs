using System.IO;
using System.Threading.Tasks;

namespace gFtp
{
    public static class DirectoryAsync
    {
        public static Task<bool> ExistsAsync(string path)
        {
            return Task.Run(() => Directory.Exists(path));
        }

        public static Task<DirectoryInfo> CreateDirectoryAsync(string path)
        {
            return Task.Run(() => Directory.CreateDirectory(path));
        }

        public static Task DeleteAsync(string path)
        {
            return Task.Run(() => Directory.Delete(path));
        }

        public static Task<DirectoryInfo> GetParentAsync(string path)
        {
            return Task.Run(() => Directory.GetParent(path));
        }

        public static Task MoveAsync(string sourcePath, string destPath)
        {
            return Task.Run(() => Directory.Move(sourcePath, destPath));
        }
    }

    public static class DirectoryInfoAsyncExtensions
    {
        public static Task<DirectoryInfo[]> GetDirectoriesAsync(this DirectoryInfo directoryInfo)
        {
            return Task.Run(() => directoryInfo.GetDirectories());
        }
    }
}
