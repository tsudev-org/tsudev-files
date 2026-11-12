// Composition root: every long-lived service is created here. Views get services through AppHost.Get<T>().

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tsudev.Files.App.Services.Triggers;
using Tsudev.Files.App.ViewModels;
using Tsudev.Files.Core.Conversion;
using Tsudev.Files.Core.Localization;
using Tsudev.Files.Core.Platform;
using Tsudev.Files.Core.Settings;
using Tsudev.Files.Core.Web;
using Tsudev.Files.Engines;
using Tsudev.Files.Engines.Infrastructure;
using Tsudev.Files.Platform.Linux;
using Tsudev.Files.Platform.MacOS;
using Tsudev.Files.Platform.Windows;

namespace Tsudev.Files.App.Services;

public static class AppHost
{
    private static IServiceProvider? _services;

    public static IServiceProvider Services => _services ?? throw new InvalidOperationException("AppHost not built.");

    public static T Get<T>() where T : notnull => Services.GetRequiredService<T>();

    public static IServiceProvider Build(string? dataDirectory = null, UserDataStore? preloadedStore = null)
    {
        var dataDir = dataDirectory ?? UserDataStore.DefaultDirectory;
        var services = new ServiceCollection();

        services.AddLogging(b => b
            .AddProvider(new FileLoggerProvider(Path.Combine(dataDir, "logs")))
            .SetMinimumLevel(LogLevel.Information));

        services.AddSingleton(sp => preloadedStore ?? new UserDataStore(dataDir, sp.GetRequiredService<ILogger<UserDataStore>>()));
        services.AddSingleton<IPlatformServices>(_ =>
            OperatingSystem.IsWindows() ? new WindowsPlatformServices()
            : OperatingSystem.IsMacOS() ? new MacOSPlatformServices()
            : OperatingSystem.IsLinux() ? new LinuxPlatformServices()
            : new NullPlatformServices());

        services.AddSingleton<LocalizationService>();
        services.AddSingleton<ILocalizer>(sp => sp.GetRequiredService<LocalizationService>());
        services.AddSingleton<ThemeService>();
        services.AddSingleton<SystemIntegrationService>();
        services.AddSingleton<ElevationService>();

        // Conversion engines
        services.AddSingleton(_ => new EngineEnvironment(dataDir));
        services.AddSingleton(sp => EngineRegistry.CreateAll(sp.GetRequiredService<EngineEnvironment>(), sp.GetRequiredService<ILoggerFactory>()));
        services.AddSingleton(sp => new ConverterCatalog(sp.GetRequiredService<IReadOnlyList<IConverter>>()));
        services.AddSingleton(sp => new JobQueue(
            sp.GetRequiredService<ConverterCatalog>(),
            EngineRegistry.FindPdfMerger(sp.GetRequiredService<IReadOnlyList<IConverter>>()),
            sp.GetRequiredService<ILogger<JobQueue>>(),
            EngineRegistry.FindFileCombiner(sp.GetRequiredService<IReadOnlyList<IConverter>>()),
            EngineRegistry.FindTiffMerger(sp.GetRequiredService<IReadOnlyList<IConverter>>()),
            EngineRegistry.FindMediaJoiner(sp.GetRequiredService<IReadOnlyList<IConverter>>()),
            sp.GetRequiredService<IPlatformServices>().MoveToTrash));

        // Files dragged from a browser with only an address are downloaded without the browser's cookies.
        services.AddSingleton(_ => new WebDownloader(userAgent: $"Filee/{typeof(AppHost).Assembly.GetName().Version?.ToString(3)}"));
        services.AddSingleton<EngineDownloadService>();
        services.AddSingleton<PresetAvailability>();
        services.AddSingleton<RadialViewModel>();
        services.AddSingleton<ConversionService>();
        services.AddSingleton<SpreadsheetPrintService>();
        services.AddSingleton<WatchFolderService>();
        services.AddSingleton<TriggerService>();
        services.AddSingleton<RadialController>();
        services.AddSingleton<StagedFiles>();
        services.AddSingleton<TrayService>();
        services.AddSingleton<UpdateService>();
        services.AddSingleton<MemoryTrimmer>();
        services.AddSingleton<WindowService>();
        services.AddSingleton<WhatsNewService>();

        _services = services.BuildServiceProvider();
        return _services;
    }
}
