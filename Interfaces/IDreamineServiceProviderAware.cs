using System;

namespace Dreamine.Hybrid.Wpf.Interfaces
{
    /// <summary>
    /// \if KO
    /// <para>루트 서비스 공급자에 접근해야 하는 애플리케이션의 계약을 정의합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Defines a contract for applications that need access to the root service provider.</para>
    /// \endif
    /// </summary>
    public interface IDreamineServiceProviderAware
    {
        /// <summary>
        /// \if KO
        /// <para>애플리케이션이 사용할 루트 서비스 공급자를 설정합니다.</para>
        /// \endif
        /// \if EN
        /// <para>Sets the root service provider used by the application.</para>
        /// \endif
        /// </summary>
        /// <param name="serviceProvider">
        /// \if KO
        /// <para>구성된 루트 서비스 공급자입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The configured root service provider.</para>
        /// \endif
        /// </param>
        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}
