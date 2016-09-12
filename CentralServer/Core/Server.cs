using System.Messaging;
using System.Threading;
using Core.Helpers;
using Topshelf;

namespace CentralServer.Core
{
    class Server : ServiceControl
    {
        private Thread _thread;
        private bool _isStopping;
        private const string CentralServerQueue = @".\private$\CentralServer";

        public bool Start(HostControl hostControl)
        {
            _thread = new Thread(ListenQueue);
            _thread.Start();

            return true;
        }

        private void ListenQueue()
        {
            QueueHelpers.EnsureQueueExists(CentralServerQueue);

            using (var queue = new MessageQueue(CentralServerQueue, true))
            {
                queue.Formatter = new XmlMessageFormatter(new[] {typeof(string)});

                while (!_isStopping)
                {
                    var message = queue.Receive();


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
