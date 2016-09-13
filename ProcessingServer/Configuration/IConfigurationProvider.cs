using System;
using System.Collections.Generic;
using Core.Data;

namespace ProcessingServer.Configuration
{
    public interface IConfigurationProvider
    {
        string CurrentDirectory { get; }
        string InDirectory { get; }
        string OutDirectory { get; }
        string FilePattern { get; }
        string NumberDelimeter { get; }
        IEnumerable<string> FileExtensions { get; }
        long ObserveInterval { get; }
        int MaxAttempts { get; }
        string ProcessingServerQueue { get; }
        string CentralServerQueue { get; }
        string ControlServerQueue { get; }

        event EventHandler ObserveIntervalChanged;

        void UpdateConfiguration(ProcessingConfiguration processingConfiguration);
    }
}
