using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Dreamine.Hybrid.Wpf.Hosting
{
    /// <summary>
    /// \if KO
    /// <para>WPF 프로세스 내부 Dreamine Blazor Server 호스팅을 위한 서비스 등록 확장을 제공합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Provides service-registration extensions for hosting Dreamine Blazor Server inside a WPF process.</para>
    /// \endif
    /// </summary>
    public static class DreamineBlazorServerServiceCollectionExtensions
    {
        /// <summary>
        /// \if KO
        /// <para>WPF 프로세스 안에서 실행될 Blazor Server 호스트를 등록합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Registers a Blazor Server host that runs inside the WPF process.</para>
        /// \endif
        /// </summary>
        /// <typeparam name="TRootComponent">
        /// \if KO
        /// <para>서버가 매핑할 루트 Razor 구성 요소 형식입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The root Razor component type mapped by the server.</para>
        /// \endif
        /// </typeparam>
        /// <param name="services">
        /// \if KO
        /// <para>호스트 서비스를 추가할 컬렉션입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The collection to which host services are added.</para>
        /// \endif
        /// </param>
        /// <param name="configure">
        /// \if KO
        /// <para>선택적 호스트 옵션 구성 콜백입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The optional host-options configuration callback.</para>
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
        public static IServiceCollection AddDreamineBlazorServer<TRootComponent>(
            this IServiceCollection services,
            Action<DreamineBlazorServerHostOptions>? configure = null)
            where TRootComponent : IComponent
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new DreamineBlazorServerHostOptions();
            configure?.Invoke(options);

            services.AddSingleton(options);
            services.AddHostedService<DreamineBlazorServerHostedService<TRootComponent>>();

            return services;
        }
    }
}
