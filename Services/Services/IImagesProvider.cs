using System.Collections.Generic;
using Services.Entities;

namespace Services.Services
{
    internal interface IImagesProvider
    {
        IEnumerable<ImageFile> GetImages();
    }
}