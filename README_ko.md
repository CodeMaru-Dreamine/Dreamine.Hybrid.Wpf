# Dreamine.Hybrid.Wpf

Dreamine 아키텍처 기반의 WPF + BlazorWebView(WebView2) 하이브리드 호스팅
인프라입니다.

[English Version](README.md)

------------------------------------------------------------------------

## 개요

Dreamine.Hybrid.Wpf는 WPF 애플리케이션 내부에 Blazor 컴포넌트를 명확한
구조로 호스팅하기 위한 프로젝트입니다.

제공 기능:

-   WPF + BlazorWebView 하이브리드 쉘 구조
-   MVVM 친화적 통합 방식
-   WPF ↔ Blazor 메시지 통신 구조
-   WebView2 수명 주기 관리
-   산업용 아키텍처 설계

------------------------------------------------------------------------

## 구조

WPF Shell │ ├── ViewModel (Dreamine MVVM) │ ├── Hybrid Host Layer │ └──
BlazorWebView (WebView2) │ └── Message Bus Bridge ↔ Blazor Components

------------------------------------------------------------------------

설계: Dreamine 대한민국
