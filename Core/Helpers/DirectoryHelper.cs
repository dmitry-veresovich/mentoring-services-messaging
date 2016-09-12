using System.IO;

namespace Core.Helpers
{
    public static class DirectoryHelper
    {
        public static void EnsureDirectoryExists(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
