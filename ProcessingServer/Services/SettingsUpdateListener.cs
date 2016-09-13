using System;
using System.Messaging;
using System.Threading;
using Core.Messages;
using ProcessingServer.Configuration;

namespace ProcessingServer.Services
{
    class SettingsUpdateListener : ISettingsUpdateListener
    {
        private const string ProcessingServerQueue = @".\private$\processingserver";
        private Thread _thread;
        private bool _isRunning;
        private readonly IConfigurationProvider _configurationProvider;

        public SettingsUpdateListener(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public void Listen()
        {
            _thread = new Thread(ListenQueue);
            _isRunning = true;
            _thread.Start();
        }

        private void ListenQueue()
        {
            while (_isRunning)
            {
                using (var queue = new MessageQueue(ProcessingServerQueue, true))
                {
                    queue.Formatter = new XmlMessageFormatter(new[] { typeof(UpdateConfigurationMessage) });

                    while (_isRunning)
                    {
                        try
                        {
                            var message = queue.Receive(TimeSpan.FromSeconds(5))?.Body as UpdateConfigurationMessage;
                            if (message == null)
                                continue;

                            _configurationProvider.UpdateConfiguration(message.ProcessingConfiguration);
                        }
                        catch (MessageQueueException) { }
                    }
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _thread.Join();
        }
    }
}
