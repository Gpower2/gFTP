using System.IO;
using System.Threading.Tasks;

namespace gFtp
{
    public static class FileAsync
    {
        public static Task<bool> ExistsAsync(string path)
        {
            return Task.Run(() => File.Exists(path));
        }
    }
}
