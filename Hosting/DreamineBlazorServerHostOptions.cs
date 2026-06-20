using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// Represents options for hosting a Blazor Server endpoint inside a WPF process.
    /// </summary>
    public sealed class DreamineBlazorServerHostOptions
    {
        /// <summary>
        /// Gets or sets the port used by the embedded Blazor Server host.
        /// </summary>
        public int Port { get; set; } = 5000;

        /// <summary>
        /// Gets the unique ID for the current in-process server instance.
        /// </summary>
        public string InstanceId { get; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets or sets a value indicating whether Kestrel should listen on every network interface.
        /// </summary>
        public bool ListenAnyIp { get; set; }

        /// <summary>
        /// Gets or sets the local host name used when <see cref="ListenAnyIp" /> is false.
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// Gets or sets the content root path used by the embedded Blazor Server host.
        /// </summary>
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        /// <summary>
        /// Gets or sets a value indicating whether public ViewModel classes should be automatically registered.
        /// </summary>
        public bool AutoRegisterViewModels { get; set; } = true;

        /// <summary>
        /// Gets service types that should be shared from the WPF host service provider to the Blazor Server host.
        /// </summary>
        public IList<Type> SharedServiceTypes { get; } = new List<Type>();

        /// <summary>
        /// Gets or sets a value indicating whether disposable shared service instances may be registered in the Blazor Server container.
        /// </summary>
        /// <remarks>
        /// Keep this disabled unless the shared instance is safe for the Blazor Server host to dispose when it stops.
        /// </remarks>
        public bool AllowDisposableSharedServices { get; set; }

        /// <summary>
        /// Optional callback invoked before the Blazor Server host is built.
        /// Use this to register Scoped or Transient services into the Blazor Server DI container.
        /// </summary>
        public Action<IServiceCollection>? ConfigureServices { get; set; }

        /// <summary>
        /// Optional callback invoked after the default pipeline (UseStaticFiles, UseRouting, UseAntiforgery)
        /// is configured. Use this to add extra middleware, static file providers, etc.
        /// </summary>
        public Action<WebApplication>? ConfigurePipeline { get; set; }

        /// <summary>
        /// Adds a physical directory as an additional static file provider at the given request path.
        /// </summary>
        public DreamineBlazorServerHostOptions AddPhysicalStaticFiles(string physicalPath, string requestPath)
        {
            var prev = ConfigurePipeline;
            ConfigurePipeline = app =>
            {
                prev?.Invoke(app);
                System.IO.Directory.CreateDirectory(physicalPath);
                app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
                {
                    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(physicalPath),
                    RequestPath = requestPath
                });
            };
            return this;
        }
    }
}
