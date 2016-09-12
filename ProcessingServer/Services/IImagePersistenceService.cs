using MigraDoc.Rendering;

namespace ProcessingServer.Services
{
    interface IImagePersistenceService
    {
        void Persist(PdfDocumentRenderer renderer);
    }
}
