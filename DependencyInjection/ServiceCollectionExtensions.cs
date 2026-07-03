// \file ServiceCollectionExtensions.cs
// Dreamine 하이브리드(WPF) 서비스 등록 확장 메서드.
// \author Dreamine
// \date 2026-01-28
// \version 1.0.0

using Dreamine.Hybrid.Interfaces;
using Dreamine.Hybrid.Messaging;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dreamine.Hybrid.Wpf.DependencyInjection
{
    /// <summary>
    /// Provides service registration extension methods for Dreamine Hybrid WPF.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the default services required to run Dreamine Hybrid WPF.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The same service collection instance for chaining.</returns>
        public static IServiceCollection AddDreamineHybridWpf(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // BlazorWebView 구동 필수 서비스.
            services.AddWpfBlazorWebView();

            // 단일 프로세스 내 WPF ↔ Embedded Blazor ↔ Hosted Blazor 통신용 메시지 버스.
            services.AddSingleton<IHybridMessageBus, InMemoryHybridMessageBus>();

#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

            return services;
        }
    }
}
