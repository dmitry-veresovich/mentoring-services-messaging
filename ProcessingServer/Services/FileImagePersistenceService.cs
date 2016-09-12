using System.IO;
using MigraDoc.Rendering;
using ProcessingServer.Configuration;

namespace ProcessingServer.Services
{
    class FileImagePersistenceService : IImagePersistenceService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public FileImagePersistenceService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void Persist(PdfDocumentRenderer renderer)
        {
            var name = Path.ChangeExtension(Path.GetRandomFileName(), "pdf");
            var path = Path.Combine(_configurationProvider.OutDirectory, name);
            renderer.Save(path);
        }
    }
}
