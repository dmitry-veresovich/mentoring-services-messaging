using System;
using System.Reflection;
using Autofac;
using Core.Messages;
using ProcessingServer.Services;

namespace ProcessingServer.Configuration
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
            builder.RegisterType<Server>().AsSelf();
            builder.RegisterType<BytesMessagesService>().As<IBytesMessagesService>();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Except<FileImagePersistenceService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            var container = builder.Build();
            return container;
        }
    }
}
