using System.Messaging;
using Core.Data;
using Core.Messages;
using ProcessingServer.Configuration;

namespace ProcessingServer.Services
{
    internal class StatusUpdateService : IStatusUpdateService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public StatusUpdateService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void UpdateStatus(StatusType statusType, string additionalInfo)
        {
            var configuration = new ProcessingConfiguration {ScanInterval = _configurationProvider.ObserveInterval };

            using (var queue = new MessageQueue(_configurationProvider.ControlServerQueue))
            {
                queue.Send(new UpdateStatusMessage
                {
                    ProcessingConfiguration = configuration,
                    StatusType = statusType,
                    AdditionalInfo = additionalInfo,
                });
            }
        }
    }
}
