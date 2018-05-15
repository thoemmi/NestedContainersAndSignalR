using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sample.Core;
using Sample.Module.Ticker;
using Sample.Module.WebApi;

namespace Sample.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) => cts.Cancel();

            Application.RootContainer = CreateContainer();

            var startables = Application.RootContainer.Resolve<IEnumerable<IApplicationStartable>>().ToList();
            foreach (var startable in startables)
            {
                startable.Start();
            }

            Console.WriteLine("Press Ctrl-C to exit");
            cts.Token.WaitHandle.WaitOne();

            Application.RootContainer.Dispose();

            Console.WriteLine("Done, press Enter");
            Console.ReadLine();
        }

        static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            // register logging
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            builder.Populate(serviceCollection);

            builder.RegisterModule<WebApiModule>();
            builder.RegisterModule<TickerModule>();

            return builder.Build();
        }
    }
}
