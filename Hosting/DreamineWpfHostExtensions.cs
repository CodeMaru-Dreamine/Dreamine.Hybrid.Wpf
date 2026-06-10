using Dreamine.Hybrid.Wpf.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// Provides extension methods for running WPF applications with a Generic Host.
    /// </summary>
    public static class DreamineWpfHostExtensions
    {
        /// <summary>
        /// Runs a WPF application using the specified Generic Host.
        /// </summary>
        /// <typeparam name="TApplication">The WPF application type.</typeparam>
        /// <param name="host">The configured Generic Host.</param>
        public static void RunDreamineWpfApp<TApplication>(this IHost host)
            where TApplication : Application, new()
        {
            if (host is null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            host.StartAsync().GetAwaiter().GetResult();

            TApplication app = new();

            if (app is IDreamineServiceProviderAware hostAwareApplication)
            {
                hostAwareApplication.SetServiceProvider(host.Services);
            }

            app.Exit += async (_, _) =>
            {
                try
                {
                    await host.StopAsync();
                }
                finally
                {
                    host.Dispose();
                }
            };

            app.Run();
        }
    }
}