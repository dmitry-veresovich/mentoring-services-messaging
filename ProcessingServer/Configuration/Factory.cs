using System;
using System.Reflection;
using Autofac;
using Castle.DynamicProxy;
using Common.Castle;
using Common.Services;
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
                .Except<StatusUpdateService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            var proxyGenerator = new ProxyGenerator();
            builder.RegisterType<StatusUpdateService>().Named<IStatusUpdateService>("statusUpdateService");
            builder.RegisterType<LoggingInterceptor>().AsSelf();
            builder.RegisterType<MethodExecutionLogger>().AsImplementedInterfaces();
            builder.RegisterDecorator<IStatusUpdateService>((c, inner) => proxyGenerator.CreateInterfaceProxyWithTarget(inner, c.Resolve<LoggingInterceptor>()), "statusUpdateService");

            var container = builder.Build();
            return container;
        }
    }
}
