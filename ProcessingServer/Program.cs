using ProcessingServer.Configuration;
using Topshelf;

namespace ProcessingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(configurator => configurator
                    .Service(Factory.Resolve<Server>)
                    .UseNLog(NLogFactory.CreateAndConfigure()));
        }
    }
}
