using System.Threading;
using ProcessingServer.Configuration;
using ProcessingServer.Helpers;
using ProcessingServer.Services;
using Topshelf;

namespace ProcessingServer
{
    internal class Server : ServiceControl
    {
        private readonly IProcessManager _processManager;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Timer _timer;
        public Server(
            IProcessManager processManager,
            IConfigurationProvider configurationProvider)
        {
            _processManager = processManager;
            _configurationProvider = configurationProvider;
            _timer = new Timer(state => _processManager.Process());
        }

        public bool Start(HostControl hostControl)
        {
            DirectoryHelper.EnsureDirectoryExists(_configurationProvider.InDirectory);
            DirectoryHelper.EnsureDirectoryExists(_configurationProvider.OutDirectory);

            _timer.Change(0, _configurationProvider.ObserveInterval);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _timer.Change(Timeout.Infinite, 0);
            _processManager.Stop();
            return true;
        }
    }
}
