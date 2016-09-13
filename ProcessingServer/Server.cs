using System;
using System.Threading;
using Core.Helpers;
using ProcessingServer.Configuration;
using ProcessingServer.Services;
using Topshelf;

namespace ProcessingServer
{
    internal class Server : ServiceControl
    {
        private readonly Timer _timer;
        private readonly IProcessManager _processManager;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISettingsUpdateListener _settingsUpdateListener;

        public Server(
            IProcessManager processManager,
            IConfigurationProvider configurationProvider,
            ISettingsUpdateListener settingsUpdateListener)
        {
            _processManager = processManager;
            _configurationProvider = configurationProvider;
            _settingsUpdateListener = settingsUpdateListener;
            _timer = new Timer(state => _processManager.Process());
            _configurationProvider.ObserveIntervalChanged += delegate
            {
                _timer.Change(0, _configurationProvider.ObserveInterval);
            };
        }

        public bool Start(HostControl hostControl)
        {
            SetUp();

            _settingsUpdateListener.Listen();

            _timer.Change(0, _configurationProvider.ObserveInterval);
            return true;
        }

        private void SetUp()
        {
            DirectoryHelper.EnsureDirectoryExists(_configurationProvider.InDirectory);
            DirectoryHelper.EnsureDirectoryExists(_configurationProvider.OutDirectory);
            QueueHelper.EnsureQueueExists(_configurationProvider.ProcessingServerQueue);
        }

        public bool Stop(HostControl hostControl)
        {
            _timer.Change(Timeout.Infinite, 0);
            _settingsUpdateListener.Stop();
            _processManager.Stop();
            return true;
        }
    }
}
