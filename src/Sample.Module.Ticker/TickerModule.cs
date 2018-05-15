using Autofac;

namespace Sample.Module.Ticker
{
    public class TickerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<TickerService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}