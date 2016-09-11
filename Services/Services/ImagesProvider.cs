using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Services.Entities;

namespace Services.Services
{
    class ImagesProvider : IImagesProvider
    {
        private readonly IConfigurationProvider _configurationProvider;
        private ICollection<string> _validExtensions;
        private ICollection<string> ValidExtensions
            => _validExtensions ?? (_validExtensions = _configurationProvider.FileExtensions.ToList());

        public ImagesProvider(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public IEnumerable<ImageFile> GetImages()
        {
            var path = Path.Combine(_configurationProvider.CurrentDirectory, _configurationProvider.InDirectory);
            var pattern = _configurationProvider.FilePattern;

            foreach (var file in Directory.EnumerateFiles(path, pattern))
            {
                if (!ExtensionApplies(file)) continue;

                var fileName = Path.GetFileNameWithoutExtension(file);
                if (string.IsNullOrWhiteSpace(fileName)) continue;

                int number;
                if (!TryGetNumber(fileName, out number)) continue;

                yield return new ImageFile(file, number);
            }
        }

        private bool TryGetNumber(string fileName, out int number)
        {
            var index = fileName.IndexOf(_configurationProvider.NumberDelimeter, StringComparison.InvariantCulture);
            var stringNumber = fileName.Substring(index + 1);
            return int.TryParse(stringNumber, out number);
        }

        private bool ExtensionApplies(string file)
        {
            var extension = Path.GetExtension(file)?.Substring(1);
            return ValidExtensions.Contains(extension);
        }
    }
}
