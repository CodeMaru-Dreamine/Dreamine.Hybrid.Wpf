// \file HybridHostControl.xaml.cs
// WPF에서 BlazorWebView를 Embedded 형태로 호스팅하는 컨트롤.
// \author Dreamine
// \date 2026-01-28
// \version 1.0.0
using Microsoft.AspNetCore.Components.WebView.Wpf;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace Dreamine.Hybrid.Wpf.Controls
{
    /// <summary>
    /// \if KO
    /// <para>WPF 안에서 Blazor UI를 포함 형태로 호스팅하는 컨트롤입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Hosts an embedded Blazor UI inside WPF.</para>
    /// \endif
    /// </summary>
    public partial class HybridHostControl : UserControl
    {
        /// <summary>
        /// \if KO
        /// <para>is Initialized 값을 보관합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Stores the is initialized value.</para>
        /// \endif
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// \if KO
        /// <para>Blazor 호스트 페이지 경로를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the Blazor host-page path.</para>
        /// \endif
        /// </summary>
        public string HostPage { get; set; } = "wwwroot/index.html";

        /// <summary>
        /// \if KO
        /// <para>마운트할 루트 Razor 구성 요소 형식을 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the root Razor component type to mount.</para>
        /// \endif
        /// </summary>
        public Type? RootComponentType { get; set; }

        /// <summary>
        /// \if KO
        /// <para>루트 구성 요소를 마운트할 CSS 선택자를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the CSS selector on which the root component is mounted.</para>
        /// \endif
        /// </summary>
        public string RootSelector { get; set; } = "#app";

        /// <summary>
        /// \if KO
        /// <para>Blazor에 제공할 서비스 공급자를 가져오거나 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Gets or sets the service provider supplied to Blazor.</para>
        /// \endif
        /// </summary>
        public IServiceProvider? Services { get; set; }

        /// <summary>
        /// \if KO
        /// <para>컨트롤을 초기화하고 런타임 Loaded 처리기를 등록합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Initializes the control and registers its runtime Loaded handler.</para>
        /// \endif
        /// </summary>
        public HybridHostControl()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            Loaded += OnLoaded;
        }

        /// <summary>
        /// \if KO
        /// <para>최초 로드 시 BlazorWebView와 루트 구성 요소를 구성합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Configures the BlazorWebView and root component on the first load.</para>
        /// \endif
        /// </summary>
        /// <param name="sender">
        /// \if KO
        /// <para>로드된 컨트롤입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The loaded control.</para>
        /// \endif
        /// </param>
        /// <param name="e">
        /// \if KO
        /// <para>라우트된 로드 이벤트 데이터입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The routed load-event data.</para>
        /// \endif
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// \if KO
        /// <para><see cref="RootComponentType"/> 또는 <see cref="Services"/>가 설정되지 않은 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when <see cref="RootComponentType"/> or <see cref="Services"/> has not been configured.</para>
        /// \endif
        /// </exception>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            if (RootComponentType == null) throw new InvalidOperationException("RootComponentType must be set.");
            if (Services == null) throw new InvalidOperationException("Services must be set.");

            BlazorView.HostPage = HostPage;
            BlazorView.Services = Services;

            BlazorView.RootComponents.Clear();
            BlazorView.RootComponents.Add(new RootComponent
			{
                Selector = RootSelector,
                ComponentType = RootComponentType
            });
        }
    }
}
