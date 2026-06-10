using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// Provides service registration extensions for Dreamine Blazor Server hosting.
    /// </summary>
    public static class DreamineBlazorServerServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a Blazor Server host that runs inside the WPF process.
        /// </summary>
        /// <typeparam name="TRootComponent">The root Razor component type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The option configuration callback.</param>
        /// <returns>The same service collection instance.</returns>
        public static IServiceCollection AddDreamineBlazorServer<TRootComponent>(
            this IServiceCollection services,
            Action<DreamineBlazorServerHostOptions>? configure = null)
            where TRootComponent : IComponent
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new DreamineBlazorServerHostOptions();
            configure?.Invoke(options);

            services.AddSingleton(options);
            services.AddHostedService<DreamineBlazorServerHostedService<TRootComponent>>();

            return services;
        }
    }
}