using System;
using System.Diagnostics;
using System.Linq;
using Core.Messages;

namespace ProcessingServer.Services
{
    internal class ProcessManager : IProcessManager
    {
        private readonly IImagesProvider _imagesProvider;
        private readonly IPdfService _pdfService;
        private readonly IImagesMoverService _imagesMoverService;
        private readonly IStatusUpdateService _statusUpdateService;
        private readonly object _lock = new object();
        private bool _isProcessing;

        public ProcessManager(
            IImagesProvider imagesProvider,
            IPdfService pdfService,
            IImagesMoverService imagesMoverService,
            IStatusUpdateService statusUpdateService)
        {
            _imagesProvider = imagesProvider;
            _pdfService = pdfService;
            _imagesMoverService = imagesMoverService;
            _statusUpdateService = statusUpdateService;
        }

        public void Process()
        {
            if (!BeginProcess())
            {
                _statusUpdateService.UpdateStatus(StatusType.Skipped);
                return;
            }

            _statusUpdateService.UpdateStatus(StatusType.Processing);

            try
            {
                ProcessImages();
            }
            catch (Exception e)
            {
                _statusUpdateService.UpdateStatus(StatusType.Error, e.ToString());
            }

            _statusUpdateService.UpdateStatus(StatusType.Idling);

            EndProcess();
        }

        private void ProcessImages()
        {
            var images = _imagesProvider.GetImages().OrderBy(file => file.Number).ToList();

            _imagesMoverService.Move(images);

            if (images.Count > 0)
            {
                _pdfService.NewFile();

                var prevNum = images[0].Number;
                foreach (var imageFile in images)
                {
                    if (imageFile.Number - prevNum > 1)
                    {
                        _pdfService.SaveFile();
                        _statusUpdateService.UpdateStatus(StatusType.GeneratedFile, $"Last file number: {imageFile.Number}");
                        _pdfService.NewFile();
                    }

                    prevNum = imageFile.Number;

                    _pdfService.AddPage(imageFile.FullPath);
                }

                _pdfService.SaveFile();
                _statusUpdateService.UpdateStatus(StatusType.GeneratedFile, $"Last file number: {prevNum}");
            }
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
