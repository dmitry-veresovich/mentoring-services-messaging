using System;
using System.Reflection;
using Autofac;

namespace ProcessingServer
{
    static class Factory
    {
        private static readonly Lazy<IContainer> Container = new Lazy<IContainer>(Init);

        public static T Resolve<T>()
        {
            return Container.Value.Resolve<T>();
        }

        private static IContainer Init()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ServiceController>().AsSelf();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            var container = builder.Build();
            return container;
        }
    }
}
