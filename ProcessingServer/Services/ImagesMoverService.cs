using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ProcessingServer.Configuration;
using ProcessingServer.Entities;

namespace ProcessingServer.Services
{
    internal class ImagesMoverService : IImagesMoverService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public ImagesMoverService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void Move(IEnumerable<ImageFile> images)
        {
            var outDir = Path.Combine(_configurationProvider.CurrentDirectory, _configurationProvider.OutDirectory);
            foreach (var imageFile in images)
            {
                int attempt = 0;
                while (attempt++ < _configurationProvider.MaxAttempts)
                {
                    try
                    {
                        var newPath = Path.Combine(outDir, imageFile.FullName);
                        File.Move(imageFile.FullPath, newPath);
                        imageFile.FullPath = newPath;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        throw;
                    }
                }
            }
        }
    }
}
