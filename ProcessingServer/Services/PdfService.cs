using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace ProcessingServer.Services
{
    class PdfService : IPdfService
    {
        private readonly IImagePersistenceService _imagePersistenceService;

        private Document _document;
        private Section _section;
        private int _pageNumber;

        public PdfService(IImagePersistenceService imagePersistenceService)
        {
            _imagePersistenceService = imagePersistenceService;
        }

        public void NewFile()
        {
            if (_document != null)
                SaveFile();

            _document = new Document();
            _section = _document.AddSection();
            _pageNumber = 1;
        }

        public void AddPage(string path)
        {
            if (_pageNumber > 1)
                _section.AddPageBreak();

            _pageNumber++;

            var img = _section.AddImage(path);
            img.LockAspectRatio = true;
            if (img.Height > _document.DefaultPageSetup.PageHeight)
                img.Height = _document.DefaultPageSetup.PageHeight;
            else if (img.Width > _document.DefaultPageSetup.PageWidth)
                img.Width = _document.DefaultPageSetup.PageWidth;
        }

        public void SaveFile()
        {
            if (_document == null)
                return;

            var renderer = new PdfDocumentRenderer {Document = _document};
            renderer.RenderDocument();
            _imagePersistenceService.Persist(renderer);

            _document = null;
        }
    }
}
