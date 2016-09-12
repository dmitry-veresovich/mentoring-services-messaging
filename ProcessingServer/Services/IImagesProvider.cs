using System.Collections.Generic;
using ProcessingServer.Entities;

namespace ProcessingServer.Services
{
    internal interface IImagesProvider
    {
        IEnumerable<ImageFile> GetImages();
    }
}