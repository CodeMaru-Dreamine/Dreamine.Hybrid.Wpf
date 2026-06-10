using Dreamine.Hybrid.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// Hosts a Blazor Server application inside the WPF process.
    /// </summary>
    /// <typeparam name="TRootComponent">The root Razor component type.</typeparam>
    public sealed class DreamineBlazorServerHostedService<TRootComponent> : IHostedService
        where TRootComponent : IComponent
    {
        private readonly IServiceProvider _rootServiceProvider;
        private readonly IHybridMessageBus _messageBus;
        private readonly DreamineBlazorServerHostOptions _options;
        private IHost? _webHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="DreamineBlazorServerHostedService{TRootComponent}"/> class.
        /// </summary>
        /// <param name="rootServiceProvider">The WPF root service provider.</param>
        /// <param name="messageBus">The shared hybrid message bus.</param>
        /// <param name="options">The Blazor Server host options.</param>
        public DreamineBlazorServerHostedService(
            IServiceProvider rootServiceProvider,
            IHybridMessageBus messageBus,
            DreamineBlazorServerHostOptions options)
        {
            _rootServiceProvider = rootServiceProvider ?? throw new ArgumentNullException(nameof(rootServiceProvider));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Assembly blazorAssembly = typeof(TRootComponent).Assembly;

            WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = Array.Empty<string>(),
                ContentRootPath = _options.ContentRootPath,
                ApplicationName = blazorAssembly.GetName().Name
            });

            StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

            builder.Services
                .AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddSingleton(_messageBus);

            RegisterSharedServices(builder.Services);

            if (_options.AutoRegisterViewModels)
            {
                RegisterViewModels(builder.Services, blazorAssembly);
            }

            builder.WebHost.ConfigureKestrel(options =>
            {
                if (_options.ListenAnyIp)
                {
                    options.ListenAnyIP(_options.Port);
                    return;
                }

                if (string.Equals(_options.Host, "localhost", StringComparison.OrdinalIgnoreCase))
                {
                    options.ListenLocalhost(_options.Port);
                    return;
                }

                if (IPAddress.TryParse(_options.Host, out IPAddress? address))
                {
                    options.Listen(address, _options.Port);
                    return;
                }

                options.ListenLocalhost(_options.Port);
            });

            WebApplication app = builder.Build();

            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                if (TryGetPublishedIndexPath(app.Environment.ContentRootPath, context.Request, out string? indexPath))
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.SendFileAsync(indexPath!);
                    return;
                }

                await next();
            });
            app.UseRouting();
            app.UseAntiforgery();

            app.MapGet("/_dreamine/instance", () => _options.InstanceId);

            app.MapRazorComponents<TRootComponent>()
               .AddInteractiveServerRenderMode();

            _webHost = app;

            await app.StartAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_webHost is null)
            {
                return;
            }

            try
            {
                await _webHost.StopAsync(cancellationToken);
            }
            finally
            {
                _webHost.Dispose();
                _webHost = null;
            }
        }

        /// <summary>
        /// Registers services shared from the WPF host service provider.
        /// </summary>
        /// <param name="services">The Blazor Server service collection.</param>
        private void RegisterSharedServices(IServiceCollection services)
        {
            foreach (Type serviceType in _options.SharedServiceTypes)
            {
                object? instance = _rootServiceProvider.GetService(serviceType);

                if (instance is null)
                {
                    throw new InvalidOperationException(
                        $"Shared service '{serviceType.FullName}' was not found in the root service provider.");
                }

                if (!_options.AllowDisposableSharedServices &&
                    (instance is IDisposable || instance is IAsyncDisposable))
                {
                    throw new InvalidOperationException(
                        $"Shared service '{serviceType.FullName}' implements IDisposable/IAsyncDisposable. " +
                        "Blazor Server would own disposal for services registered into its container. " +
                        "Share a non-disposable facade or explicitly enable AllowDisposableSharedServices when that ownership is intended.");
                }

                services.AddSingleton(serviceType, instance);
            }
        }

        /// <summary>
        /// Registers public ViewModel classes from the specified assembly.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assembly">The assembly to scan.</param>
        private static void RegisterViewModels(IServiceCollection services, Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes()
                         .Where(type =>
                             type.IsClass &&
                             !type.IsAbstract &&
                             type.IsPublic &&
                             type.Name.EndsWith("ViewModel", StringComparison.Ordinal)))
            {
                services.AddScoped(type);
            }
        }

        private static bool TryGetPublishedIndexPath(string contentRootPath, HttpRequest request, out string? indexPath)
        {
            indexPath = null;

            if (!HttpMethods.IsGet(request.Method) && !HttpMethods.IsHead(request.Method))
            {
                return false;
            }

            string? requestPath = request.Path.Value;
            if (string.IsNullOrWhiteSpace(requestPath) ||
                string.Equals(requestPath, "/", StringComparison.Ordinal) ||
                !requestPath.EndsWith("/", StringComparison.Ordinal))
            {
                return false;
            }

            string[] segments = requestPath
                .Trim('/')
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (segments.Length == 0 ||
                segments.Any(segment => segment is "." or ".." || Path.GetFileName(segment) != segment))
            {
                return false;
            }

            string webRoot = Path.GetFullPath(Path.Combine(contentRootPath, "wwwroot"));
            string targetDirectory = webRoot;
            foreach (string segment in segments)
            {
                targetDirectory = Path.Combine(targetDirectory, segment);
            }

            string candidate = Path.GetFullPath(Path.Combine(targetDirectory, "index.html"));
            string webRootPrefix = webRoot.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
                ? webRoot
                : webRoot + Path.DirectorySeparatorChar;

            if (!candidate.StartsWith(webRootPrefix, StringComparison.OrdinalIgnoreCase) || !File.Exists(candidate))
            {
                return false;
            }

            indexPath = candidate;
            return true;
        }
    }
}
