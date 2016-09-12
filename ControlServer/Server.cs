using Topshelf;

namespace ControlServer
{
    class Server : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}
