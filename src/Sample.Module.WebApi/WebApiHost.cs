using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Sample.Core;

namespace Sample.Module.WebApi
{
    public class WebApiHost : IApplicationStartable
    {
        public void Start()
        {
            CreateWebHostBuilder()
                .Build()
                .Start();
        }

        private static IWebHostBuilder CreateWebHostBuilder() =>
            WebHost.CreateDefaultBuilder()
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>();

    }
}
