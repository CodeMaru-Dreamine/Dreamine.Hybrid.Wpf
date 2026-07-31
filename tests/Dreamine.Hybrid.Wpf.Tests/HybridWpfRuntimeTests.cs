using System.Globalization;
using System.IO;
using System.Windows;
using Dreamine.Hybrid.Interfaces;
using Dreamine.Hybrid.Messaging;
using Dreamine.Hybrid.Wpf.Converters;
using Dreamine.Hybrid.Wpf.DependencyInjection;
using Dreamine.Hybrid.Wpf.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Dreamine.Hybrid.Wpf.Tests;

public sealed class HybridWpfRuntimeTests
{
    [Theory]
    [InlineData(true, Visibility.Visible)]
    [InlineData(false, Visibility.Collapsed)]
    public void Converter_maps_boolean_to_visibility(bool value, Visibility expected)
    {
        var actual = BooleanToVisibilityConverter.Instance.Convert(
            value,
            typeof(Visibility),
            parameter: null!,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(Visibility.Visible, true)]
    [InlineData(Visibility.Hidden, false)]
    [InlineData(Visibility.Collapsed, false)]
    public void Converter_maps_visibility_to_boolean(Visibility value, bool expected)
    {
        var actual = BooleanToVisibilityConverter.Instance.ConvertBack(
            value,
            typeof(bool),
            parameter: null!,
            CultureInfo.InvariantCulture);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Converter_rejects_unrelated_values()
    {
        Assert.Equal(
            Visibility.Collapsed,
            BooleanToVisibilityConverter.Instance.Convert(
                "true",
                typeof(Visibility),
                parameter: null!,
                CultureInfo.InvariantCulture));

        Assert.Equal(
            false,
            BooleanToVisibilityConverter.Instance.ConvertBack(
                "Visible",
                typeof(bool),
                parameter: null!,
                CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Hybrid_registration_adds_singleton_message_bus()
    {
        var services = new ServiceCollection();

        var result = services.AddDreamineHybridWpf();
        var registration = Assert.Single(
            services,
            descriptor => descriptor.ServiceType == typeof(IHybridMessageBus));

        Assert.Same(services, result);
        Assert.Equal(typeof(InMemoryHybridMessageBus), registration.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, registration.Lifetime);
    }

    [Fact]
    public void Hybrid_registration_rejects_null_collection()
    {
        IServiceCollection services = null!;

        Assert.Throws<ArgumentNullException>(() => services.AddDreamineHybridWpf());
    }

    [Fact]
    public void Server_registration_preserves_options_and_hosted_service()
    {
        var services = new ServiceCollection();

        var result = services.AddDreamineBlazorServer<TestComponent>(options =>
        {
            options.Port = 6123;
            options.ListenAnyIp = true;
            options.Host = "example.test";
            options.AutoRegisterViewModels = false;
            options.SharedServiceTypes.Add(typeof(IHybridMessageBus));
        });

        Assert.Same(services, result);
        var options = Assert.Single(
            services,
            descriptor => descriptor.ServiceType == typeof(DreamineBlazorServerHostOptions));
        var value = Assert.IsType<DreamineBlazorServerHostOptions>(options.ImplementationInstance);
        Assert.Equal(6123, value.Port);
        Assert.True(value.ListenAnyIp);
        Assert.Equal("example.test", value.Host);
        Assert.False(value.AutoRegisterViewModels);
        Assert.Contains(typeof(IHybridMessageBus), value.SharedServiceTypes);
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IHostedService));
    }

    [Fact]
    public void Server_registration_rejects_null_collection()
    {
        IServiceCollection services = null!;

        Assert.Throws<ArgumentNullException>(
            () => services.AddDreamineBlazorServer<TestComponent>());
    }

    [Fact]
    public void Host_options_start_with_safe_defaults()
    {
        var options = new DreamineBlazorServerHostOptions();

        Assert.Equal(5000, options.Port);
        Assert.Equal("localhost", options.Host);
        Assert.False(options.ListenAnyIp);
        Assert.True(options.AutoRegisterViewModels);
        Assert.True(options.UseEmbeddedWebView);
        Assert.False(options.AllowDisposableSharedServices);
        Assert.Empty(options.SharedServiceTypes);
        Assert.Equal(32, options.InstanceId.Length);
    }

    [Fact]
    public void Host_options_add_physical_static_files_to_pipeline()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"dreamine-hybrid-{Guid.NewGuid():N}");
        var previousCallbackInvoked = false;
        var options = new DreamineBlazorServerHostOptions
        {
            ConfigurePipeline = _ => previousCallbackInvoked = true,
        };

        try
        {
            var result = options.AddPhysicalStaticFiles(directory, "/assets");
            var app = WebApplication.CreateBuilder().Build();

            options.ConfigurePipeline!(app);

            Assert.Same(options, result);
            Assert.True(previousCallbackInvoked);
            Assert.True(Directory.Exists(directory));
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }

    private sealed class TestComponent : ComponentBase;
}
