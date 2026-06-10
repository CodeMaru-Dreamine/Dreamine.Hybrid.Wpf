using Dreamine.Hybrid.Wpf.Internal;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Threading.Tasks;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// Provides the public WebView2 host helpers supported by Dreamine Hybrid WPF.
    /// </summary>
    public static class HybridWebViewHost
    {
        /// <summary>
        /// Creates a WebView2 instance configured for Dreamine Hybrid hosting.
        /// </summary>
        /// <returns>The configured WebView2 instance.</returns>
        public static WebView2 CreateWebView()
        {
            return WebView2Initializer.CreateConfiguredWebView2();
        }

        /// <summary>
        /// Displays an offline message in the specified WebView2 instance.
        /// </summary>
        /// <param name="webView">The WebView2 instance.</param>
        /// <param name="url">The URL that could not be reached.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
