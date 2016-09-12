using System.Collections;
using System.Collections.Generic;
using ProcessingServer.Entities;

namespace ProcessingServer.Services
{
    internal interface IImagesMoverService
    {
        void Move(IEnumerable<ImageFile> images);
    }
}
