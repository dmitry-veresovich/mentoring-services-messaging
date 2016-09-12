using MigraDoc.Rendering;

namespace ProcessingServer.Services
{
    internal interface IImagePersistenceService
    {
        void Persist(PdfDocumentRenderer renderer);
    }
}
