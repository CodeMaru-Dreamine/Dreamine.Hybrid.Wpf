using Dreamine.Hybrid.Wpf.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// \if KO
    /// <para>Generic Host와 함께 WPF 애플리케이션을 실행하는 확장 메서드를 제공합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Provides extensions for running WPF applications with a Generic Host.</para>
    /// \endif
    /// </summary>
    public static class DreamineWpfHostExtensions
    {
        /// <summary>
        /// \if KO
        /// <para>Software Rendering Environment Variable 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the software rendering environment variable value.</para>
        /// \endif
        /// </summary>
        private const string SoftwareRenderingEnvironmentVariable = "DREAMINE_WPF_SOFTWARE_RENDERING";

        /// <summary>
        /// \if KO
        /// <para>Generic Host를 시작하고 WPF 애플리케이션을 실행한 뒤 종료 시 호스트를 정리합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Starts the Generic Host, runs the WPF application, and cleans up the host when the application exits.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TApplication">
        /// \if KO
        /// <para>기본 생성자를 가진 WPF 애플리케이션 형식입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The WPF application type with a default constructor.</para>
        /// \endif
        /// </typeparam>
        /// <param name="host">
        /// \if KO
        /// <para>구성된 Generic Host입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The configured Generic Host.</para>
        /// \endif
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para><paramref name="host"/>가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when <paramref name="host"/> is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
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

        /// <summary>
        /// \if KO
        /// <para>환경 변수 요청에 따라 프로세스 렌더링 모드를 소프트웨어 전용으로 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Sets the process rendering mode to software-only when requested by the environment variable.</para>
        /// \endif
        /// </summary>
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
