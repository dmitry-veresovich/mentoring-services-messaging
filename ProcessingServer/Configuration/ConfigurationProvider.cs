using System;
using System.Collections.Generic;
using Core.Data;
using ProcessingServer.Helpers;

namespace ProcessingServer.Configuration
{
    internal class ConfigurationProvider : IConfigurationProvider
    {
        public string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public string InDirectory => "in";
        public string OutDirectory => "out";
        public string FilePattern => "img_*.*";
        public string NumberDelimeter => "_";
        public IEnumerable<string> FileExtensions => new [] { "jpg" };
        public long ObserveInterval { get; private set; } = 5000;
        public int MaxAttempts => 3;
        public string ProcessingServerQueue => @".\private$\processingserver";
        public string CentralServerQueue => @".\private$\centralserver";
        public string ControlServerQueue => @".\private$\controlserver";

        public event EventHandler ObserveIntervalChanged = delegate {};

        public void UpdateConfiguration(ProcessingConfiguration processingConfiguration)
        {
            Guard.NotNull(processingConfiguration, nameof(processingConfiguration));

            ObserveInterval = processingConfiguration.ScanInterval;
            ObserveIntervalChanged(this, EventArgs.Empty);
        }
    }
}
