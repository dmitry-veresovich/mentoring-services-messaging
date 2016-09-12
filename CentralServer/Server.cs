using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Threading;
using Core.Helpers;
using Core.Messages;
using Topshelf;

namespace CentralServer
{
    class Server : ServiceControl
    {
        private Thread _thread;
        private bool _isStopping;
        private const string CentralServerQueue = @".\private$\centralserver";
        private readonly Dictionary<Guid, List<SplitMessage<byte[]>>> _chunks = new Dictionary<Guid, List<SplitMessage<byte[]>>>();
        private readonly IBytesMessagesService _bytesMessagesService;
        private static string OutDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "out");

        public Server(IBytesMessagesService bytesMessagesService)
        {
            _bytesMessagesService = bytesMessagesService;
        }

        public bool Start(HostControl hostControl)
        {
            _thread = new Thread(ListenQueue);
            _thread.Start();

            return true;
        }

        private void ListenQueue()
        {
            QueueHelper.EnsureQueueExists(CentralServerQueue);
            DirectoryHelper.EnsureDirectoryExists(OutDir);

            using (var queue = new MessageQueue(CentralServerQueue, true))
            {
                queue.Formatter = new XmlMessageFormatter(new[] {typeof(SplitMessage<byte[]>)});

                while (!_isStopping)
                {
                    try
                    {
                        var message = queue.Receive(TimeSpan.FromSeconds(5))?.Body as SplitMessage<byte[]>;
                        if (message == null)
                            continue;

                        var receivedMessages = GetReceivedMessages(message);
                        receivedMessages.Add(message);
                        var size = message.Size;
                        if (size == receivedMessages.Count)
                        {
                            var bytes = _bytesMessagesService.Collect(receivedMessages);

                            SaveImage(bytes);

                            _chunks.Remove(message.Id);
                        }

                    }
                    catch (MessageQueueException) { }
                }
            }
        }

        private void SaveImage(byte[] bytes)
        {
            var name = Path.ChangeExtension(Path.GetRandomFileName(), "pdf");
            var path = Path.Combine(OutDir, name);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        private List<SplitMessage<byte[]>> GetReceivedMessages(SplitMessage<byte[]> body)
        {
            List<SplitMessage<byte[]>> receivedMessages;
            if (_chunks.ContainsKey(body.Id))
            {
                receivedMessages = _chunks[body.Id];
            }
            else
            {
                receivedMessages = new List<SplitMessage<byte[]>>();
                _chunks.Add(body.Id, receivedMessages);
            }

            return receivedMessages;
        }

        public bool Stop(HostControl hostControl)
        {
            _isStopping = true;
            _thread.Join();
            return true;
        }
    }
}
