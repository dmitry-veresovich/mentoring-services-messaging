using System;
using System.Diagnostics;
using System.IO;
using System.Messaging;
using System.Threading;
using System.Xml.Serialization;
using ControlServer.Services;
using Core.Data;
using Core.Helpers;
using Core.Messages;
using Topshelf;

namespace ControlServer
{
    class Server : ServiceControl
    {
        private Thread _listenQueueThread;
        private bool _isStopping;
        private const string ControlServerQueue = @".\private$\controlserver";
        private const string SettingsFile = @"settings.xml";
        private FileSystemWatcher _fileWatcher;
        private readonly IUpdateStatusLogService _updateStatusLogService;
        private readonly ISettingsUpdateNotifyService _settingsUpdateNotifyService;

        public Server(
            IUpdateStatusLogService updateStatusLogService,
            ISettingsUpdateNotifyService settingsUpdateNotifyService)
        {
            _updateStatusLogService = updateStatusLogService;
            _settingsUpdateNotifyService = settingsUpdateNotifyService;
        }

        public bool Start(HostControl hostControl)
        {
            _listenQueueThread = new Thread(ListenQueue);
            _listenQueueThread.Start();

            _fileWatcher = new FileSystemWatcher
            {
                Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory),
                Filter = SettingsFile,
                NotifyFilter = NotifyFilters.LastWrite,
            };
            _fileWatcher.Changed += (sender, args) =>
            {
                try
                {
                    using (var fileStream = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFile), FileMode.Open))
                    {
                        var serializer = new XmlSerializer(typeof(ProcessingConfiguration));
                        var settigns = serializer.Deserialize(fileStream) as ProcessingConfiguration;

                        _settingsUpdateNotifyService.NotifyConfigurationChanged(settigns);
                    }
                }
                catch (Exception e)
                {
                    Debug.Write(e);
                }
            };
            _fileWatcher.EnableRaisingEvents = true;

            return true;
        }

        private void ListenQueue()
        {
            QueueHelper.EnsureQueueExists(ControlServerQueue);

            using (var queue = new MessageQueue(ControlServerQueue, true))
            {
                queue.Formatter = new XmlMessageFormatter(new[] { typeof(UpdateStatusMessage) });

                while (!_isStopping)
                {
                    try
                    {
                        var message = queue.Receive(TimeSpan.FromSeconds(5))?.Body as UpdateStatusMessage;
                        if (message == null)
                            continue;

                        _updateStatusLogService.LogStatus(message);
                    }
                    catch (MessageQueueException) { }
                }
            }
        }

        public bool Stop(HostControl hostControl)
        {
            _isStopping = true;
            _listenQueueThread.Join();
            _fileWatcher.EnableRaisingEvents = false;
            return true;
        }
    }
}
