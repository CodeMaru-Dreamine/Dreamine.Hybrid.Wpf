using Dreamine.Hybrid.Wpf.Internal;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Threading.Tasks;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// \if KO
    /// <para>Dreamine Hybrid WPF가 지원하는 공개 WebView2 호스트 도우미를 제공합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Provides the public WebView2 host helpers supported by Dreamine Hybrid WPF.</para>
    /// \endif
    /// </summary>
    public static class HybridWebViewHost
    {
        /// <summary>
        /// \if KO
        /// <para>안전한 캐시 경로와 브라우저 인수를 적용한 WebView2 인스턴스를 만듭니다.</para>
        /// \endif
        /// \if EN
        /// <para>Creates a WebView2 instance with a safe cache path and configured browser arguments.</para>
        /// \endif
        /// </summary>
        /// <returns>
        /// \if KO
        /// <para>구성된 WebView2 인스턴스입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The configured WebView2 instance.</para>
        /// \endif
        /// </returns>
        public static WebView2 CreateWebView()
        {
            return WebView2Initializer.CreateConfiguredWebView2();
        }

        /// <summary>
        /// \if KO
        /// <para>지정한 WebView2에 서버 오프라인 안내 HTML을 표시합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Displays server-offline HTML in the specified WebView2 instance.</para>
        /// \endif
        /// </summary>
        /// <param name="webView">
        /// \if KO
        /// <para>안내를 표시할 WebView2입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The WebView2 in which to display the message.</para>
        /// \endif
        /// </param>
        /// <param name="url">
        /// \if KO
        /// <para>연결할 수 없었던 URL입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The URL that could not be reached.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>안내 표시 작업입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A task representing display of the message.</para>
        /// \endif
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para><paramref name="webView"/>가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when <paramref name="webView"/> is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
        public static Task ShowOfflineMessageAsync(WebView2 webView, string url)
        {
            if (webView is null)
            {
                throw new ArgumentNullException(nameof(webView));
            }

            return WebView2Initializer.ShowOfflineMessageAsync(webView, url);
        }
    }
}
