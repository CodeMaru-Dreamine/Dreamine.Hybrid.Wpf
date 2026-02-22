/// \file WebView2Initializer.cs
/// \brief WebView2 초기화/캐시 경로/진단 유틸리티.
/// \details 다국어·특수문자 경로 문제를 회피하기 위해 ASCII 전용 LocalAppData 하위 경로를 사용.
/// \author Dreamine
/// \version 1.0.0

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Dreamine.Hybrid.Wpf.Internal
{
	/// \class WebView2Initializer
	/// \brief WebView2 안전 초기화를 제공하는 정적 유틸 클래스
	public static class WebView2Initializer
	{
		/// \brief WebView2 캐시 경로를 반환(ASCII Only)
		public static string GetSafeUserDataFolder()
		{
			var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var path = Path.Combine(basePath, "Dreamine", "WebView2Cache"); // e.g., C:\Users\...\AppData\Local\Dreamine\WebView2Cache
			Directory.CreateDirectory(path);
			return path;
		}

		/// \brief 캐시 경로가 지정된 WebView2 인스턴스를 생성
		public static WebView2 CreateConfiguredWebView2()
		{
			var cachePath = GetSafeUserDataFolder();
			Debug.WriteLine($"[WebView2.CachePath] {cachePath}");

			var webView = new WebView2
			{
				CreationProperties = new CoreWebView2CreationProperties
				{
					UserDataFolder = cachePath
				}
			};

			webView.CoreWebView2InitializationCompleted += (s, a) =>
			{
				if (!a.IsSuccess)
					Debug.WriteLine($"[WebView2.InitFailed] {a.InitializationException}");
				else
					Debug.WriteLine($"[WebView2.InitOK] {webView.CoreWebView2?.Environment?.BrowserVersionString}");
			};

			webView.NavigationStarting += (s, e) => Debug.WriteLine($"[WebView2.NavStarting] {e.Uri}");
			webView.NavigationCompleted += (s, e) =>
			{
				if (!e.IsSuccess)
					Debug.WriteLine($"[WebView2.NavFailed] {e.WebErrorStatus}");
				else
					Debug.WriteLine("[WebView2.NavOK]");
			};

			return webView;
		}

		/// \brief 서버 오프라인 안내 HTML을 로드
		public static async Task ShowOfflineMessageAsync(WebView2 webView, string url)
		{
			await webView.EnsureCoreWebView2Async();
			webView.NavigateToString($@"
<!doctype html>
<html>
<head><meta charset='utf-8'><title>Server Offline</title></head>
<body style='font-family:Segoe UI; background:#222; color:#ddd; padding:24px;'>
  <h2>Blazor Server에 연결할 수 없습니다.</h2>
  <p>대상: <b>{System.Net.WebUtility.HtmlEncode(url)}</b></p>
  <ul>
    <li>Kestrel이 지정 포트에서 기동 중인지 확인(UseUrls)</li>
    <li>다른 프로세스가 포트를 점유하지 않는지 확인</li>
    <li>로컬 방화벽/보안 솔루션 차단 해제</li>
  </ul>
</body>
</html>");
		}
	}
}
