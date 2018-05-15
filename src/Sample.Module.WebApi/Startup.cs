using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Sample.Core;

namespace Sample.Module.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddSignalR();
            services.AddConnections();

            var scope = Application.RootContainer.BeginLifetimeScope(b => b.Populate(services));
            return new AutofacServiceProvider(scope);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // index page and js are embedded resources
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new EmbeddedFileProvider(GetType().Assembly, GetType().Namespace + ".wwwroot")
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<NotificationsHub>("/notifications");
            });

            app.UseMvc();
        }
    }
}
