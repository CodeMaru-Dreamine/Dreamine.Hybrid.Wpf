/// \file ServiceCollectionExtensions.cs
/// \brief Dreamine 하이브리드(WPF) 서비스 등록 확장 메서드.
/// \author Dreamine
/// \date 2026-01-28
/// \version 1.0.0
using Dreamine.Hybrid.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebView.Wpf;
namespace Dreamine.Hybrid.Wpf.DependencyInjection
{
    /// <summary>서비스 등록 확장 메서드 모음입니다.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 하이브리드 UI 구동에 필요한 기본 서비스를 등록합니다.
        /// </summary>
        /// <param name="services">서비스 컬렉션</param>
        /// <returns>체이닝을 위한 services</returns>
        public static IServiceCollection AddDreamineHybridWpf(this IServiceCollection services)
        {
            // BlazorWebView 구동을 위한 필수 서비스
            services.AddWpfBlazorWebView();

            // 코어 메시지 버스 (단일 프로세스 공유)
            services.AddSingleton<IHybridMessageBus, InMemoryHybridMessageBus>();

            // Blazor Dashboard VM
            services.AddSingleton<Dreamine.Hybrid.BlazorApp.ViewModel.IndexViewModel>();

            return services;
        }
    }
}
