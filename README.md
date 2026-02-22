# Dreamine.Hybrid.Wpf

Hybrid hosting infrastructure for WPF + BlazorWebView (WebView2),
designed for Dreamine architecture.

[한국어 문서 보기](README_ko.md)

------------------------------------------------------------------------

## Overview

Dreamine.Hybrid.Wpf provides a structured and explicit way to host
Blazor components inside a WPF application using WebView2.

This repository demonstrates:

-   WPF + BlazorWebView hybrid shell
-   Explicit MVVM-friendly integration
-   Message bus communication between WPF and Blazor
-   Clean hosting lifecycle management
-   Production-ready WebView2 configuration

------------------------------------------------------------------------

## Architecture

WPF Shell │ ├── ViewModel (Dreamine MVVM) │ ├── Hybrid Host Layer │ └──
BlazorWebView (WebView2) │ └── Message Bus Bridge ↔ Blazor Components

------------------------------------------------------------------------

Designed by Dreamine Republic of Korea
