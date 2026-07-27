using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// \if KO
    /// <para>WPF 프로세스 내부에 Blazor Server 엔드포인트를 호스팅하는 옵션을 나타냅니다.</para>
    /// \endif
    /// \if EN
    /// <para>Represents options for hosting a Blazor Server endpoint inside a WPF process.</para>
    /// \endif
    /// </summary>
    public sealed class DreamineBlazorServerHostOptions
    {
        /// <summary>
        /// \if KO
        /// <para>포함된 Blazor Server 호스트의 수신 포트를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the listening port used by the embedded Blazor Server host.</para>
        /// \endif
        /// </summary>
        public int Port { get; set; } = 5000;

        /// <summary>
        /// \if KO
        /// <para>현재 프로세스 내부 서버 인스턴스의 고유 ID를 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the unique ID of the current in-process server instance.</para>
        /// \endif
        /// </summary>
        public string InstanceId { get; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// \if KO
        /// <para>Kestrel이 모든 네트워크 인터페이스에서 수신할지 여부를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets whether Kestrel listens on every network interface.</para>
        /// \endif
        /// </summary>
        public bool ListenAnyIp { get; set; }

        /// <summary>
        /// \if KO
        /// <para><see cref="P:Dreamine.Hybrid.Wpf.Hosting.DreamineBlazorServerHostOptions.ListenAnyIp" />가 <see langword="false" />일 때 사용할 호스트 이름을 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the host name used when <see cref="P:Dreamine.Hybrid.Wpf.Hosting.DreamineBlazorServerHostOptions.ListenAnyIp" /> is <see langword="false" />.</para>
        /// \endif
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// \if KO
        /// <para>포함된 Blazor Server 호스트의 콘텐츠 루트 경로를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the content-root path used by the embedded Blazor Server host.</para>
        /// \endif
        /// </summary>
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        /// <summary>
        /// \if KO
        /// <para>공개 ViewModel 클래스를 자동 등록할지 여부를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets whether public ViewModel classes are registered automatically.</para>
        /// \endif
        /// </summary>
        public bool AutoRegisterViewModels { get; set; } = true;

        /// <summary>
        /// \if KO
        /// <para>WPF 루트 공급자에서 Blazor Server 호스트로 공유할 서비스 형식 목록을 가져옵니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets the service types shared from the WPF root provider to the Blazor Server host.</para>
        /// \endif
        /// </summary>
        public IList<Type> SharedServiceTypes { get; } = new List<Type>();

        /// <summary>
        /// \if KO
        /// <para>해제 가능한 공유 서비스 인스턴스를 Blazor Server 컨테이너에 등록할 수 있는지 여부를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets whether disposable shared-service instances may be registered in the Blazor Server container.</para>
        /// \endif
        /// </summary>
        /// <remarks>
        /// \if KO
        /// <para>해제 가능한 공유 서비스 인스턴스를 Blazor Server 컨테이너에 등록할 수 있는지 여부를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets whether disposable shared-service instances may be registered in the Blazor Server container.</para>
        /// \endif
        /// </remarks>
        public bool AllowDisposableSharedServices { get; set; }

        /// <summary>
        /// \if KO
        /// <para>WPF 셸이 포함된 WebView2 컨트롤을 호스팅할지 여부를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets whether the WPF shell hosts an embedded WebView2 control.</para>
        /// \endif
        /// </summary>
        /// <remarks>
        /// \if KO
        /// <para>WPF 셸이 포함된 WebView2 컨트롤을 호스팅할지 여부를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets whether the WPF shell hosts an embedded WebView2 control.</para>
        /// \endif
        /// </remarks>
        public bool UseEmbeddedWebView { get; set; } = true;

        /// <summary>
        /// \if KO
        /// <para>Blazor Server 호스트를 빌드하기 전에 호출되는 선택적 콜백을 가져오거나 설정합니다. 서버 DI 컨테이너에 Scoped 또는 Transient 서비스를 등록하는 데 사용합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets an optional callback invoked before the Blazor Server host is built, used to register scoped or transient services in the server container.</para>
        /// \endif
        /// </summary>
        public Action<IServiceCollection>? ConfigureServices { get; set; }

        /// <summary>
        /// \if KO
        /// <para>기본 파이프라인이 구성된 뒤 호출되는 선택적 콜백을 가져오거나 설정합니다. 추가 미들웨어나 정적 파일 공급자를 등록하는 데 사용합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets an optional callback invoked after the default pipeline is configured, used to add middleware or static-file providers.</para>
        /// \endif
        /// </summary>
        public Action<WebApplication>? ConfigurePipeline { get; set; }

        /// <summary>
        /// \if KO
        /// <para>UseRouting 직후와 UseAntiforgery 전에 호출되는 선택적 콜백을 가져오거나 설정합니다. UseAuthorization 같은 엔드포인트 라우팅 의존 미들웨어를 추가합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets an optional callback invoked immediately after UseRouting and before UseAntiforgery for endpoint-routing-dependent middleware such as UseAuthorization.</para>
        /// \endif
        /// </summary>
        public Action<WebApplication>? ConfigurePipelineAfterRouting { get; set; }

        /// <summary>
        /// \if KO
        /// <para>물리 디렉터리를 지정한 요청 경로의 추가 정적 파일 공급자로 등록합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Adds a physical directory as an additional static-file provider at the specified request path.</para>
        /// \endif
        /// </summary>
        /// <param name="physicalPath">
        /// \if KO
        /// <para>제공할 물리 디렉터리 경로입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The physical directory path to serve.</para>
        /// \endif
        /// </param>
        /// <param name="requestPath">
        /// \if KO
        /// <para>정적 파일을 노출할 요청 경로입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The request path on which files are exposed.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>연속 구성을 위한 현재 옵션 인스턴스입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The current options instance for chaining.</para>
        /// \endif
        /// </returns>
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
