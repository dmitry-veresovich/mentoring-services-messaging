using System.IO;

namespace ProcessingServer.Helpers
{
    static class DirectoryHelper
    {
        public static void EnsureDirectoryExists(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
