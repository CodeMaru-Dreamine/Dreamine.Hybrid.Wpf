using Dreamine.Hybrid.Wpf.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// Provides extension methods for running WPF applications with a Generic Host.
    /// </summary>
    public static class DreamineWpfHostExtensions
    {
        private const string SoftwareRenderingEnvironmentVariable = "DREAMINE_WPF_SOFTWARE_RENDERING";

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

            ConfigureWpfRendering();
            host.StartAsync().GetAwaiter().GetResult();

            TApplication app = new();

            if (app is IDreamineServiceProviderAware hostAwareApplication)
            {
                hostAwareApplication.SetServiceProvider(host.Services);
            }

            try
            {
                app.Run();
            }
            finally
            {
                try
                {
                    using var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(10));
                    host.StopAsync(cts.Token).GetAwaiter().GetResult();
                }
                finally
                {
                    host.Dispose();
                }
            }
        }

        private static void ConfigureWpfRendering()
        {
            var softwareRendering = Environment.GetEnvironmentVariable(SoftwareRenderingEnvironmentVariable);
            if (!string.Equals(softwareRendering, "1", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(softwareRendering, "true", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        }
    }
}
