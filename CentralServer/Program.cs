﻿using Topshelf;

namespace CentralServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(configurator => configurator.Service<Server>());
        }
    }
}
