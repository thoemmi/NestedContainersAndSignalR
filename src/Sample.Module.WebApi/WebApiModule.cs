using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Internal;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sample.Core;

namespace Sample.Module.WebApi
{
    public class WebApiModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<WebApiHost>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<SignalRNotificationService>()
                .As<INotificationService>()
                .InstancePerDependency();

            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddSignalR();
            //builder.Populate(serviceCollection);

            builder.RegisterType<HubOptionsSetup>().As(typeof(IConfigureOptions<HubOptions>)).SingleInstance();
            builder.RegisterGeneric(typeof(DefaultHubLifetimeManager<>)).As(typeof(HubLifetimeManager<>)).SingleInstance();
            builder.RegisterType(typeof(DefaultHubProtocolResolver)).As(typeof(IHubProtocolResolver)).SingleInstance();
            builder.RegisterGeneric(typeof(HubConnectionHandler<>)).As(typeof(HubConnectionHandler<>)).SingleInstance();
            builder.RegisterType(typeof(DefaultUserIdProvider)).As(typeof(IUserIdProvider)).SingleInstance();
            builder.RegisterGeneric(typeof(DefaultHubDispatcher<>)).As(typeof(HubDispatcher<>)).SingleInstance();

            var assembly = Assembly.Load("Microsoft.AspNetCore.SignalR.Core");
            builder.RegisterGeneric(assembly.GetType("Microsoft.AspNetCore.SignalR.Internal.HubContext`1", true)).As(typeof(IHubContext<>)).SingleInstance();
            builder.RegisterGeneric(assembly.GetType("Microsoft.AspNetCore.SignalR.Internal.HubContext`2", true)).As(typeof(IHubContext<,>)).SingleInstance();
            builder.RegisterType(assembly.GetType("Microsoft.AspNetCore.SignalR.Internal.SignalRMarkerService", true)).SingleInstance();

            builder.Configure<HubOptions>(o =>
            {
                o.EnableDetailedErrors = true;
            });

            builder.RegisterType(typeof(JsonHubProtocol)).As(typeof(IHubProtocol)).SingleInstance();
            builder.Configure<JsonHubProtocolOptions>();

            builder.RegisterGeneric(typeof(DefaultHubActivator<>)).As(typeof(IHubActivator<>)).InstancePerLifetimeScope();
        }
    }
}