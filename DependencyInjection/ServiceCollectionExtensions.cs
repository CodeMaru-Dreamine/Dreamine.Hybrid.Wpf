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
    /// \if KO
    /// <para>Dreamine Hybrid WPF 서비스 등록 확장 메서드를 제공합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Provides service-registration extensions for Dreamine Hybrid WPF.</para>
    /// \endif
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// \if KO
        /// <para>Dreamine Hybrid WPF 실행에 필요한 BlazorWebView와 메시지 버스 서비스를 등록합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers the BlazorWebView and message-bus services required to run Dreamine Hybrid WPF.</para>
        /// \endif
        /// </summary>
        /// <param name="services">
        /// \if KO
        /// <para>서비스를 추가할 컬렉션입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The collection to which services are added.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>연속 구성을 위한 동일한 서비스 컬렉션입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The same service collection for chaining.</para>
        /// \endif
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// \if KO
        /// <para><paramref name="services"/>가 <see langword="null"/>인 경우 발생합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Thrown when <paramref name="services"/> is <see langword="null"/>.</para>
        /// \endif
        /// </exception>
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
