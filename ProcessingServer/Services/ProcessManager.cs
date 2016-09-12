using System;
using System.Diagnostics;
using System.Linq;

namespace ProcessingServer.Services
{
    internal class ProcessManager : IProcessManager
    {
        private readonly IImagesProvider _imagesProvider;
        private readonly IPdfService _pdfService;
        private readonly IImagesMoverService _imagesMoverService;
        private readonly object _lock = new object();
        private bool _isProcessing;

        public ProcessManager(
            IImagesProvider imagesProvider,
            IPdfService pdfService,
            IImagesMoverService imagesMoverService)
        {
            _imagesProvider = imagesProvider;
            _pdfService = pdfService;
            _imagesMoverService = imagesMoverService;
        }

        public void Process()
        {
            if (!BeginProcess()) return;

            var images = _imagesProvider.GetImages().OrderBy(file => file.Number).ToList();

            _imagesMoverService.Move(images);

            if (images.Count > 0)
            {
                _pdfService.NewFile();

                var prevNum = images[0].Number;
                foreach (var imageFile in images)
                {
                    if (imageFile.Number - prevNum > 1)
                        _pdfService.NewFile();

                    prevNum = imageFile.Number;

                    _pdfService.AddPage(imageFile.FullPath);
                }

                _pdfService.SaveFile();
            }

            EndProcess();
        }

        private bool BeginProcess()
        {
            lock (_lock)
            {
                if (_isProcessing)
                    return false;

                _isProcessing = true;
            }
            Debug.WriteLine($"{DateTime.Now} - Started");
            return true;
        }

        private void EndProcess()
        {
            lock (_lock)
            {
                _isProcessing = false;
            }
            Debug.WriteLine($"{DateTime.Now} - Ended");
        }

        public void Stop()
        {
            while (_isProcessing) { }
        }
    }
}
