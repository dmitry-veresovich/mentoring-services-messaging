using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;

namespace ProcessingServer.Services
{
    class PdfService : IPdfService
    {
        private readonly IConfigurationProvider _configurationProvider;

        private Document _document;
        private Section _section;
        private int _pageNumber;

        public PdfService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
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

            var render = new PdfDocumentRenderer {Document = _document};
            render.RenderDocument();
            var name = Path.ChangeExtension(Path.GetRandomFileName(), "pdf");
            var path = Path.Combine(_configurationProvider.OutDirectory, name);
            render.Save(path);

            _document = null;
        }
    }
}
