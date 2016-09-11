using System.IO;

namespace Services.Helpers
{
    static class DirectoryHelper
    {
        public static void EnsureDirectoryExists(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
