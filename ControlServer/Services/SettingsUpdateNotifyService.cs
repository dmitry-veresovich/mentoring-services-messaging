using System.Messaging;
using Core.Data;
using Core.Messages;

namespace ControlServer.Services
{
    class SettingsUpdateNotifyService : ISettingsUpdateNotifyService
    {
        private readonly string[] _processingServerQueues = { @".\private$\processingserver" };

        public void NotifyConfigurationChanged(ProcessingConfiguration configuration)
        {
            foreach (var processingServerQueue in _processingServerQueues)
            {
                using (var queue = new MessageQueue(processingServerQueue))
                {
                    queue.Send(new UpdateConfigurationMessage
                    {
                        ProcessingConfiguration = configuration,
                    });
                }
            }
        }
    }
}
