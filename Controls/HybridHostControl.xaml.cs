/// \file HybridHostControl.xaml.cs
/// \brief WPF에서 BlazorWebView를 Embedded 형태로 호스팅하는 컨트롤.
/// \author Dreamine
/// \date 2026-01-28
/// \version 1.0.0
using Microsoft.AspNetCore.Components.WebView.Wpf;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace Dreamine.Hybrid.Wpf.Controls
{
    /// <summary>WPF에서 Blazor UI를 Embedded 형태로 호스팅하는 컨트롤입니다.</summary>
    public partial class HybridHostControl : UserControl
    {
        private bool _isInitialized;

        /// <summary>Blazor HostPage 경로입니다.</summary>
        public string HostPage { get; set; } = "wwwroot/index.html";

        /// <summary>RootComponent 타입입니다.</summary>
        public Type? RootComponentType { get; set; }

        /// <summary>RootComponent가 마운트될 Selector입니다.</summary>
        public string RootSelector { get; set; } = "#app";

        /// <summary>Blazor에 제공할 서비스 컨테이너입니다.</summary>
        public IServiceProvider? Services { get; set; }

        /// <summary>컨트롤을 생성합니다.</summary>
        public HybridHostControl()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            Loaded += OnLoaded;
        }

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
