using System;
using System.Messaging;
using System.Threading;
using Core.Helpers;
using Topshelf;

namespace CentralServer
{
    class Server : ServiceControl
    {
        private Thread _thread;
        private bool _isStopping;
        private const string CentralServerQueue = @".\private$\centralserver";

        public bool Start(HostControl hostControl)
        {
            _thread = new Thread(ListenQueue);
            _thread.Start();

            return true;
        }

        private void ListenQueue()
        {
            QueueHelper.EnsureQueueExists(CentralServerQueue);

            using (var queue = new MessageQueue(CentralServerQueue, true))
            {
                queue.Formatter = new XmlMessageFormatter(new[] {typeof(string)});

                while (!_isStopping)
                {
                    try
                    {
                        var message = queue.Receive(TimeSpan.FromSeconds(5));
                        
                    }
                    catch (MessageQueueException) { }
                }
            }
        }

        public bool Stop(HostControl hostControl)
        {
            _isStopping = true;
            _thread.Join();
            return true;
        }
    }
}
