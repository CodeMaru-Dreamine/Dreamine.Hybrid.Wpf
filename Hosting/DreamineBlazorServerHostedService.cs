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
    /// \if KO
    /// <para>WPF 프로세스 내부에서 Blazor Server 애플리케이션의 생명 주기를 호스팅합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Hosts the lifecycle of a Blazor Server application inside the WPF process.</para>
    /// \endif
    /// </summary>
    /// <typeparam name="TRootComponent">
    /// \if KO
    /// <para>매핑할 루트 Razor 구성 요소 형식입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The root Razor component type to map.</para>
    /// \endif
    /// </typeparam>
    public sealed class DreamineBlazorServerHostedService<TRootComponent> : IHostedService
        where TRootComponent : IComponent
    {
        /// <summary>
        /// \if KO
        /// <para>root Service Provider 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the root service provider value.</para>
        /// \endif
        /// </summary>
        private readonly IServiceProvider _rootServiceProvider;
        /// <summary>
        /// \if KO
        /// <para>message Bus 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the message bus value.</para>
        /// \endif
        /// </summary>
        private readonly IHybridMessageBus _messageBus;
        /// <summary>
        /// \if KO
        /// <para>options 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the options value.</para>
        /// \endif
        /// </summary>
        private readonly DreamineBlazorServerHostOptions _options;
        /// <summary>
        /// \if KO
        /// <para>web Host 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the web host value.</para>
        /// \endif
        /// </summary>
        private IHost? _webHost;

        /// <summary>
        /// \if KO
        /// <para>루트 서비스 공급자, 공유 메시지 버스 및 호스트 옵션으로 새 호스팅 서비스를 초기화합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes a new hosted service with the root service provider, shared message bus, and host options.</para>
        /// \endif
        /// </summary>
        /// <param name="rootServiceProvider">
        /// \if KO
        /// <para>WPF 루트 서비스 공급자입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The WPF root service provider.</para>
        /// \endif
        /// </param>
        /// <param name="messageBus">
        /// \if KO
        /// <para>공유 하이브리드 메시지 버스입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The shared hybrid message bus.</para>
        /// \endif
        /// </param>
        /// <param name="options">
        /// \if KO
        /// <para>Blazor Server 호스트 옵션입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The Blazor Server host options.</para>
        /// \endif
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para>인수 중 하나가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when any argument is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public DreamineBlazorServerHostedService(
            IServiceProvider rootServiceProvider,
            IHybridMessageBus messageBus,
            DreamineBlazorServerHostOptions options)
        {
            _rootServiceProvider = rootServiceProvider ?? throw new ArgumentNullException(nameof(rootServiceProvider));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// \if KO
        /// <para>Kestrel, Razor 구성 요소, 공유 서비스 및 미들웨어를 구성하고 내부 웹 호스트를 시작합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Configures Kestrel, Razor components, shared services, and middleware, then starts the internal web host.</para>
        /// \endif
        /// </summary>
        /// <param name="cancellationToken">
        /// \if KO
        /// <para>호스트 시작 취소 토큰입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A token used to cancel host startup.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>내부 웹 호스트 시작 작업입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A task representing startup of the internal web host.</para>
        /// \endif
        /// </returns>
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
            _options.ConfigureServices?.Invoke(builder.Services);

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
                    await context.Response.SendFileAsync(
                        indexPath!,
                        context.RequestAborted);
                    return;
                }

                await next();
            });

            // 추가 정적 파일(업로드 데이터 등)을 UseRouting 이전에 등록해야
            // Blazor catch-all 라우터보다 먼저 처리됩니다.
            _options.ConfigurePipeline?.Invoke(app);

            app.UseRouting();

            _options.ConfigurePipelineAfterRouting?.Invoke(app);

            app.UseAntiforgery();

            app.MapGet("/_dreamine/instance", () => _options.InstanceId);

            app.MapRazorComponents<TRootComponent>()
               .AddInteractiveServerRenderMode();

            _webHost = app;

            await app.StartAsync(cancellationToken);
        }

        /// <summary>
        /// \if KO
        /// <para>실행 중인 내부 웹 호스트를 중지하고 해제합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stops and disposes the running internal web host.</para>
        /// \endif
        /// </summary>
        /// <param name="cancellationToken">
        /// \if KO
        /// <para>호스트 중지 취소 토큰입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A token used to cancel host shutdown.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>내부 웹 호스트 중지 작업입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A task representing shutdown of the internal web host.</para>
        /// \endif
        /// </returns>
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
        /// \if KO
        /// <para>WPF 루트 공급자의 구성된 서비스 인스턴스를 Blazor Server 컨테이너에 공유합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Shares configured service instances from the WPF root provider with the Blazor Server container.</para>
        /// \endif
        /// </summary>
        /// <param name="services">
        /// \if KO
        /// <para>공유 서비스를 등록할 Blazor Server 서비스 컬렉션입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The Blazor Server service collection receiving shared services.</para>
        /// \endif
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// \if KO
        /// <para>공유 서비스가 루트 공급자에 없거나 허용되지 않은 해제 가능 서비스인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when a shared service is absent from the root provider or is a disallowed disposable service.</para>
        /// \endif
        /// </exception>
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
        /// \if KO
        /// <para>지정한 어셈블리의 공개 비추상 ViewModel 클래스를 Scoped 서비스로 등록합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers public, non-abstract ViewModel classes from the specified assembly as scoped services.</para>
        /// \endif
        /// </summary>
        /// <param name="services">
        /// \if KO
        /// <para>ViewModel을 등록할 서비스 컬렉션입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The service collection receiving the ViewModels.</para>
        /// \endif
        /// </param>
        /// <param name="assembly">
        /// \if KO
        /// <para>검색할 어셈블리입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The assembly to scan.</para>
        /// \endif
        /// </param>
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

        /// <summary>
        /// \if KO
        /// <para>안전한 디렉터리형 GET 또는 HEAD 요청을 게시된 wwwroot/index.html 파일에 매핑합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Maps a safe directory-style GET or HEAD request to a published wwwroot/index.html file.</para>
        /// \endif
        /// </summary>
        /// <param name="contentRootPath">
        /// \if KO
        /// <para>애플리케이션 콘텐츠 루트입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The application content root.</para>
        /// \endif
        /// </param>
        /// <param name="request">
        /// \if KO
        /// <para>검사할 HTTP 요청입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The HTTP request to inspect.</para>
        /// \endif
        /// </param>
        /// <param name="indexPath">
        /// \if KO
        /// <para>성공하면 확인된 index.html 절대 경로입니다.</para>
        /// \endif
        /// \if EN
        /// <para>When successful, receives the verified absolute index.html path.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>안전하고 존재하는 인덱스 파일을 찾았는지 여부입니다.</para>
        /// \endif
        /// \if EN
        /// <para>Whether a safe, existing index file was found.</para>
        /// \endif
        /// </returns>
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
