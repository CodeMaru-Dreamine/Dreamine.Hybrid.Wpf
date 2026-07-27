// \file WebView2Initializer.cs
// WebView2 초기화/캐시 경로/진단 유틸리티.
// \details 다국어·특수문자 경로 문제를 회피하기 위해 ASCII 전용 LocalAppData 하위 경로를 사용.
// \author Dreamine
// \version 1.0.0

using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Dreamine.Hybrid.Wpf.Internal
{
	/// <summary>
	/// \if KO
	/// <para>안전한 사용자 데이터 경로와 저자원 브라우저 구성을 사용하여 WebView2를 초기화합니다.</para>
	/// \endif
	/// \if EN
	/// <para>Initializes WebView2 with a safe user-data path and low-resource browser configuration.</para>
	/// \endif
	/// </summary>
	internal static class WebView2Initializer
	{
		/// <summary>
		/// \if KO
		/// <para>Low Resource Mode Environment Variable 값을 보관합니다.</para>
		/// \endif
		/// \if EN
		/// <para>Stores the low resource mode environment variable value.</para>
		/// \endif
		/// </summary>
		private const string LowResourceModeEnvironmentVariable = "DREAMINE_WEBVIEW2_LOW_RESOURCE_MODE";

		/// <summary>
		/// \if KO
		/// <para>현재 프로세스와 권한 수준별 WebView2 사용자 데이터 디렉터리를 만들고 반환합니다.</para>
		/// \endif
		/// \if EN
		/// <para>Creates and returns a WebView2 user-data directory scoped by process and integrity level.</para>
		/// \endif
		/// </summary>
		/// <returns>
		/// \if KO
		/// <para>생성이 보장된 사용자 데이터 디렉터리 경로입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The path of the ensured user-data directory.</para>
		/// \endif
		/// </returns>
		public static string GetSafeUserDataFolder()
		{
			var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var processName = Process.GetCurrentProcess().ProcessName;
			var integrity = IsAdministrator() ? "Admin" : "User";
			var path = Path.Combine(basePath, "Dreamine", "WebView2Cache", processName, integrity);
			Directory.CreateDirectory(path);
			return path;
		}

		/// <summary>
		/// \if KO
		/// <para>환경 변수 설정에 따라 저자원 WebView2 브라우저 인수를 만듭니다.</para>
		/// \endif
		/// \if EN
		/// <para>Builds low-resource WebView2 browser arguments according to the environment setting.</para>
		/// \endif
		/// </summary>
		/// <returns>
		/// \if KO
		/// <para>추가 브라우저 인수 문자열이며 저자원 모드가 꺼졌으면 빈 문자열입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The additional browser-argument string, or an empty string when low-resource mode is disabled.</para>
		/// \endif
		/// </returns>
		private static string GetAdditionalBrowserArguments()
		{
			var lowResourceMode = Environment.GetEnvironmentVariable(LowResourceModeEnvironmentVariable);
			if (!string.Equals(lowResourceMode, "1", StringComparison.OrdinalIgnoreCase) &&
				!string.Equals(lowResourceMode, "true", StringComparison.OrdinalIgnoreCase))
			{
				return string.Empty;
			}

			return string.Join(" ", new[]
			{
				"--disable-gpu",
				"--disable-gpu-compositing",
				"--disable-accelerated-2d-canvas",
				"--disable-accelerated-video-decode",
				"--disable-smooth-scrolling",
				"--disable-features=CalculateNativeWinOcclusion,msWebOOUI,msPdfOOUI"
			});
		}

		/// <summary>
		/// \if KO
		/// <para>현재 Windows ID가 관리자 역할에 속하는지 안전하게 확인합니다.</para>
		/// \endif
		/// \if EN
		/// <para>Safely determines whether the current Windows identity belongs to the administrator role.</para>
		/// \endif
		/// </summary>
		/// <returns>
		/// \if KO
		/// <para>관리자이면 <see langword="true"/>, 확인 실패 또는 일반 사용자이면 <see langword="false"/>입니다.</para>
		/// \endif
		/// \if EN
		/// <para><see langword="true"/> for an administrator; <see langword="false"/> for a standard user or when detection fails.</para>
		/// \endif
		/// </returns>
		private static bool IsAdministrator()
		{
			try
			{
				using WindowsIdentity identity = WindowsIdentity.GetCurrent();
				return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// \if KO
		/// <para>안전한 캐시 경로, 브라우저 인수 및 진단 이벤트를 적용한 WebView2를 만듭니다.</para>
		/// \endif
		/// \if EN
		/// <para>Creates a WebView2 with a safe cache path, browser arguments, and diagnostic events.</para>
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
		public static WebView2 CreateConfiguredWebView2()
		{
			var cachePath = GetSafeUserDataFolder();
			var browserArguments = GetAdditionalBrowserArguments();
			Debug.WriteLine($"[WebView2.CachePath] {cachePath}");
			Debug.WriteLine($"[WebView2.Args] {(string.IsNullOrWhiteSpace(browserArguments) ? "(none)" : browserArguments)}");

			var webView = new WebView2
			{
				CreationProperties = new CoreWebView2CreationProperties
				{
					UserDataFolder = cachePath,
					AdditionalBrowserArguments = browserArguments
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

		/// <summary>
		/// \if KO
		/// <para>WebView2 초기화를 보장하고 HTML 인코딩된 대상 URL과 함께 오프라인 안내를 표시합니다.</para>
		/// \endif
		/// \if EN
		/// <para>Ensures WebView2 initialization and displays an offline message containing the HTML-encoded target URL.</para>
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
		/// <para>연결하지 못한 대상 URL입니다.</para>
		/// \endif
		/// \if EN
		/// <para>The target URL that could not be reached.</para>
		/// \endif
		/// </param>
		/// <returns>
		/// \if KO
		/// <para>WebView2 초기화 및 안내 표시 작업입니다.</para>
		/// \endif
		/// \if EN
		/// <para>A task representing WebView2 initialization and message display.</para>
		/// \endif
		/// </returns>
		/// <remarks>
		/// \if KO
		/// <para>WebView2 오류는 UI 흐름을 중단하지 않도록 진단 출력에 기록하고 내부에서 처리합니다.</para>
		/// \endif
		/// \if EN
		/// <para>WebView2 failures are written to diagnostic output and handled internally so they do not interrupt the UI flow.</para>
		/// \endif
		/// </remarks>
		public static async Task ShowOfflineMessageAsync(WebView2 webView, string url)
		{
			try
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
			catch (Exception ex)
			{
				Debug.WriteLine($"[WebView2.OfflineMessageFailed] {ex}");
			}
		}
	}
}
