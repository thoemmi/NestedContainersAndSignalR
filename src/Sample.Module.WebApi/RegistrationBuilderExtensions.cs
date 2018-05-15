using System;
using Autofac;
using Microsoft.Extensions.Options;

namespace Sample.Module.WebApi
{
    public static class RegistrationBuilderExtensions
    {
        /// <summary>
        /// Registers an action used to configure a particular type of options.
        /// </summary>
        /// <typeparam name="TOptions">The options type to be configured.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder" /> to add the services to.</param>
        /// <param name="configureOptions">The action used to configure the options.</param>
        /// <returns>The <see cref="ContainerBuilder" /> so that additional calls can be chained.</returns>
        public static ContainerBuilder Configure<TOptions>(this ContainerBuilder builder, Action<TOptions> configureOptions = null) where TOptions : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder
                .Register<IConfigureOptions<TOptions>>(ctx => new ConfigureOptions<TOptions>(configureOptions))
                .SingleInstance();

            return builder;
        }
    }
}