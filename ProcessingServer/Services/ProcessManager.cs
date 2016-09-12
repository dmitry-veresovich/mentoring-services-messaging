using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ProcessingServer.Entities;

namespace ProcessingServer.Services
{
    class ProcessManager : IProcessManager
    {
        private readonly IImagesProvider _imagesProvider;
        private readonly IPdfService _pdfService;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly object _lock = new object();
        private bool _isProcessing;

        public ProcessManager(
            IImagesProvider imagesProvider,
            IPdfService pdfService,
            IConfigurationProvider configurationProvider)
        {
            _imagesProvider = imagesProvider;
            _pdfService = pdfService;
            _configurationProvider = configurationProvider;
        }

        public void Process()
        {
            if (!BeginProcess()) return;

            var images = _imagesProvider.GetImages().OrderBy(file => file.Number).ToList();

            MoveImages(images);

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

        private void MoveImages(IEnumerable<ImageFile> images)
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

        private void EndProcess()
        {
            lock (_lock)
            {
                _isProcessing = false;
            }
            Debug.WriteLine($"{DateTime.Now} - Ended");
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

        public void Stop()
        {
            while (_isProcessing) { }
        }
    }
}
