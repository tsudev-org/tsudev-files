// TsuFiles: kiểu giao diện "TsuFiles Cú đêm" (ADR-0003) - bề mặt đen tuyền + tím của logo, màu nhấn cố định, đủ tương
// phản, mặc định cho bản cài mới và không đổi lựa chọn của người đã cài.

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Styling;
using TsuFiles.App.Services;
using TsuFiles.App.ViewModels;
using TsuFiles.App.ViewModels.Pages;
using TsuFiles.App.Views;
using TsuFiles.App.Views.Pages;
using TsuFiles.Core.Localization;
using TsuFiles.Core.Platform;
using TsuFiles.Core.Settings;

namespace TsuFiles.App.Tests;

public class NightOwlThemeTests
{
    private static SolidColorBrush Resolve(string key, ThemeVariant variant)
    {
        var window = new Window();
        Assert.True(window.TryFindResource(key, variant, out var value), $"{key} ({variant})");
        return Assert.IsType<SolidColorBrush>(value);
    }

    [AvaloniaFact]
    public void Night_owl_is_pitch_black_with_the_owl_purple_whatever_accent_was_picked()
    {
        TestServices.EnsureInitialized("en");
        var theme = AppHost.Get<ThemeService>();
        try
        {
            theme.Apply(new ThemeSettings { Style = ThemeStyle.NightOwl, Accent = "#1D9E75", PresetKey = "mint" }, "en");
            Assert.True(theme.IsNightOwl);
            Assert.False(theme.IsMaterialYou);
            Assert.Equal(Color.Parse(NightOwlPalette.DarkPage), Resolve("PageBrush", ThemeVariant.Dark).Color);
            Assert.Equal(Color.Parse(NightOwlPalette.DarkAccent), Resolve("AccentBrush", ThemeVariant.Dark).Color);
            Assert.Equal(Color.Parse(NightOwlPalette.LightAccent), Resolve("AccentBrush", ThemeVariant.Light).Color);
        }
        finally
        {
            theme.Apply(new ThemeSettings(), "en");
        }
    }

    [AvaloniaFact]
    public void Night_owl_text_and_accent_labels_meet_wcag_aa_in_both_variants()
    {
        TestServices.EnsureInitialized("en");
        var theme = AppHost.Get<ThemeService>();
        try
        {
            theme.Apply(new ThemeSettings { Style = ThemeStyle.NightOwl }, "en");
            foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            {
                var page = Resolve("PageBrush", variant).Color;
                var surface = Resolve("SurfaceBrush", variant).Color;
                foreach (var background in new[] { page, surface })
                {
                    Assert.True(SoftPalette.ContrastRatio(Resolve("TextPrimaryBrush", variant).Color, background) >= 7, $"text {variant}");
                    Assert.True(SoftPalette.ContrastRatio(Resolve("TextSecondaryBrush", variant).Color, background) >= 4.5, $"secondary {variant}");
                }
                var accent = Resolve("AccentBrush", variant).Color;
                foreach (var background in new[] { page, surface })
                {
                    Assert.True(SoftPalette.ContrastRatio(accent, background) >= 4.5, $"accent text {variant}");
                    Assert.True(SoftPalette.ContrastRatio(Resolve("AccentStrongBrush", variant).Color, background) >= 4.5, $"strong {variant}");
                }
                Assert.True(SoftPalette.ContrastRatio(Resolve("OnAccentBrush", variant).Color, accent) >= 4.5, $"on-accent {variant}");
                Assert.Equal(surface, Resolve("ButtonBackground", variant).Color);
            }
        }
        finally
        {
            theme.Apply(new ThemeSettings(), "en");
        }
    }

    [AvaloniaFact]
    public void Theme_json_colours_still_override_night_owl()
    {
        TestServices.EnsureInitialized("en");
        var theme = AppHost.Get<ThemeService>();
        try
        {
            var custom = new CustomTheme { Dark = new Dictionary<string, string> { ["page"] = "#101820" } };
            theme.Apply(new ThemeSettings { Style = ThemeStyle.NightOwl }, "en", custom);
            Assert.Equal(Color.Parse("#101820"), Resolve("PageBrush", ThemeVariant.Dark).Color);
        }
        finally
        {
            theme.Apply(new ThemeSettings(), "en");
        }
    }

    [Fact]
    public void A_new_install_starts_in_night_owl_dark_and_an_existing_one_keeps_its_style()
    {
        var fresh = Directory.CreateTempSubdirectory("tsufiles-owl-").FullName;
        var existing = Directory.CreateTempSubdirectory("tsufiles-owl-").FullName;
        try
        {
            var store = new UserDataStore(fresh);
            store.Load();
            Assert.Equal(ThemeStyle.NightOwl, store.Settings.Theme.Style);
            Assert.Equal(ThemeMode.Dark, store.Settings.Theme.Mode);

            File.WriteAllText(Path.Combine(existing, "settings.json"), """{ "theme": { "style": "Soft", "mode": "System" } }""");
            var old = new UserDataStore(existing);
            old.Load();
            Assert.Equal(ThemeStyle.Soft, old.Settings.Theme.Style);
            Assert.Equal(ThemeMode.System, old.Settings.Theme.Mode);
        }
        finally
        {
            Directory.Delete(fresh, true);
            Directory.Delete(existing, true);
        }
    }

    [AvaloniaFact]
    public void Theme_page_offers_night_owl_and_hides_the_accent_picker_for_it()
    {
        TestServices.EnsureInitialized("en");
        var directory = Directory.CreateTempSubdirectory("tsufiles-owl-vm-").FullName;
        var store = new UserDataStore(directory);
        store.Load();
        try
        {
            var vm = new ThemePageViewModel(store, AppHost.Get<ILocalizer>(), AppHost.Get<IPlatformServices>());
            Assert.Contains(vm.Styles, s => s.Value == ThemeStyle.NightOwl && s.Label == "TsuFiles Night Owl");
            Assert.True(vm.IsNightOwl);
            Assert.False(vm.AccentIsCustomizable);

            vm.SetStyleCommand.Execute(ThemeStyle.Soft);
            Assert.True(vm.IsSoft);
            Assert.True(vm.AccentIsCustomizable);
            Assert.Equal(ThemeStyle.Soft, store.Settings.Theme.Style);

            vm.SetStyleCommand.Execute(ThemeStyle.NightOwl);
            Assert.Equal(ThemeStyle.NightOwl, store.Settings.Theme.Style);
        }
        finally
        {
            AppHost.Get<ThemeService>().Apply(new ThemeSettings(), "en");
            Directory.Delete(directory, true);
        }
    }

    // Ảnh chụp cửa sổ chính như người dùng mới thấy (Cú đêm tối) cho README và website:
    // bin/.../screenshots/readme-<trang>-<ngôn ngữ>.png, chép sang assets/screenshots/ (xem assets/screenshots/README.md).
    [AvaloniaTheory]
    [InlineData("home", "en")]
    [InlineData("home", "vi")]
    [InlineData("toolbar", "en")]
    [InlineData("toolbar", "vi")]
    public void Main_window_renders_in_night_owl_dark_for_the_readme(string page, string language)
    {
        TestServices.EnsureInitialized(language);
        var store = AppHost.Get<UserDataStore>();
        var previous = store.Settings.Theme;
        var theme = AppHost.Get<ThemeService>();
        store.Settings.Theme = new ThemeSettings { Style = ThemeStyle.NightOwl, Mode = ThemeMode.Dark };
        theme.Apply(store.Settings.Theme, language);
        using var vm = new MainWindowViewModel(AppHost.Services, AppHost.Get<ILocalizer>(), AppHost.Get<UpdateService>(), store);
        vm.Navigate(page);
        var window = new MainWindow { DataContext = vm, Width = 1080, Height = 720 };
        window.Show();
        try
        {
            RenderTests.Pump();
            Assert.Equal(ThemeVariant.Dark, window.ActualThemeVariant);
            RenderTests.Save(window, $"readme-{page}-{language}.png");
        }
        finally
        {
            window.Close();
            store.Settings.Theme = previous;
            theme.Apply(previous, "en");
            TestServices.EnsureInitialized("en");
        }
    }

    // Chụp trang Giao diện ở Cú đêm tối (bin/.../screenshots/night-owl-theme-page.png) và kiểm nền trang là đen tuyền.
    [AvaloniaFact]
    public void Theme_page_renders_in_night_owl_dark()
    {
        TestServices.EnsureInitialized("vi");
        var directory = Directory.CreateTempSubdirectory("tsufiles-owl-shot-").FullName;
        var store = new UserDataStore(directory);
        store.Load();
        var theme = AppHost.Get<ThemeService>();
        theme.Apply(store.Settings.Theme, "vi");
        var vm = new ThemePageViewModel(store, AppHost.Get<ILocalizer>(), AppHost.Get<IPlatformServices>());
        var window = new Window
        {
            Width = 1100,
            Height = 800,
            Background = Resolve("PageBrush", ThemeVariant.Dark),
            Content = new ThemePage { DataContext = vm },
        };
        window.Show();
        try
        {
            RenderTests.Pump();
            using var frame = window.CaptureRenderedFrame()!;
            var path = Path.Combine(TestServices.ScreenshotDirectory, "night-owl-theme-page.png");
            using (var stream = File.Create(path))
                frame.Save(stream, new Avalonia.Media.Imaging.PngBitmapEncoderOptions());
            Assert.True(File.Exists(path));
            Assert.Equal(ThemeVariant.Dark, window.ActualThemeVariant);
        }
        finally
        {
            window.Close();
            theme.Apply(new ThemeSettings(), "en");
            TestServices.EnsureInitialized("en");
            Directory.Delete(directory, true);
        }
    }
}
