Sample application to demonstrate using SignalR in nested Autofac containers.

## Projects 

The solution consists of 4 projects:

### Sample.Core

* `INotificationService` defines a service to send notifications inside the application.
* `IApplicationStartable` describes components which will be started when the application
  starts.
* `Application` holds a reference to the root Autofac container

### Sample.Module.Ticker

Implements the startable `TickerService`, which sends a message via `INotificationService` every second.

### Sample.Module.WebApi

* Implements `WebApiHost`, which starts a web server. It also serves a simple web page,
  which subscribes to SignalR.
* `SignalRNotificationService` implements `INotificationService`, forwarding every message 
  to clients via SignalR.

### Sample.Service

The actual executable. It configures the Autofac container, invokes all `IApplicationStartable`,
and will wait until the user presses <kbd>Ctrl</kbd>+<kbd>C</kbd>.

## The challenge

`Startup.ConfigureServices` is called when the Autofac container is already build. So
the services registered in the method cannt be added to the root container but in a
child "web" container.

However, `SignalRNotificationService` must be available in the root container,
so it can be injected into other components from other modules, which do not run
in the web context. However, this demands that `HubLifetimeManager<>` is available
too, plus all registrations it depends on. Because `HubContext<>` and `HubContext<,>`
are internal classes of SignalR, I use reflection to access them (another option is to
copy the sources of those classes into my project). Additionally, SignalR
requires a marker service, which is internal as well. Therefore the registration 
looks like this:

```csharp
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

        // registrations required by SignalR
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
```