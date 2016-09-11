using System.IO;

namespace Services.Entities
{
    class ImageFile
    {
        public ImageFile(string path, int number)
        {
            FullPath = path;
            Number = number;
        }

        public string FullPath { get; set; }
        public string Name => Path.GetFileNameWithoutExtension(FullPath);
        public string FullName => Path.GetFileName(FullPath);
        public int Number { get; set; }
    }
}
