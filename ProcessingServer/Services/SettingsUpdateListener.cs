using System.Threading;

namespace ProcessingServer.Services
{
    class SettingsUpdateListener : ISettingsUpdateListener
    {
        private Thread _thread;
        private bool _isRunning;

        public void Listen()
        {
            _thread= new Thread(ListenQueue);
        }

        private void ListenQueue()
        {
            while (_isRunning)
            {
                
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _thread.Join();
        }
    }
}
