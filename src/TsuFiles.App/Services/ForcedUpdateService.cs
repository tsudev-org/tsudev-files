// TsuFiles: bắt buộc cập nhật khi mở ứng dụng (chuẩn hệ sinh thái, phiếu 20261112-04; lõi ở Core/Updates/VersionGate.cs).
// Bản thấp hơn "toiThieu" của files.tsudev.com/phien-ban.json: dừng trình kích hoạt nền và thư mục theo dõi, hiện hộp
// thoại không đóng được với "Tải bản mới" (mở trang tải cố định) và "Thoát". Bản cũ hơn "moiNhat" nhưng còn dùng được:
// không hiện thêm gì - thẻ cập nhật sẵn có (UpdateService, nhắc một lần mỗi phiên bản, có "Để sau") đã lo phần đó.

using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Microsoft.Extensions.Logging;
using TsuFiles.Core;
using TsuFiles.Core.Localization;
using TsuFiles.Core.Settings;
using TsuFiles.Core.Updates;

namespace TsuFiles.App.Services;

public sealed class ForcedUpdateService(UserDataStore store, ILocalizer loc, ILogger<ForcedUpdateService> log)
{
    private Window? _dialog;

    /// <summary>Đang bị chặn (đã hiện hộp thoại bắt buộc cập nhật).</summary>
    public bool IsBlocked { get; private set; }

    /// <summary>Gọi khi bị chặn để dừng mọi dịch vụ nền (App gắn TriggerService, WatchFolderService).</summary>
    public Action? StopBackgroundServices { get; set; }

    /// <summary>Thoát ứng dụng (App gắn App.Quit).</summary>
    public Action? Quit { get; set; }

    /// <summary>
    /// Kiểm khi mở: chặn ngay theo mức tối thiểu đã lưu (kể cả khi mất mạng), rồi tải tệp phiên bản nếu người dùng bật
    /// kiểm tra cập nhật. <paramref name="handler"/> chỉ dành cho test.
    /// </summary>
    public async Task<VersionGateResult> CheckAsync(HttpMessageHandler? handler = null, Uri? url = null,
        CancellationToken cancellationToken = default)
    {
        var settings = store.Settings;
        if (VersionGate.Decide(ForkDefaults.BuildNumber, null, settings.UpdateMinimumBuild) == VersionGateResult.Required)
        {
            Block(settings.UpdateMinimumBuild, version: null);
            return VersionGateResult.Required;
        }
        if (!settings.CheckForUpdates)
            return VersionGateResult.Unknown;

        var file = await VersionGate.FetchAsync(handler, url, cancellationToken);
        var raised = VersionGate.RaiseMinimum(settings.UpdateMinimumBuild, file);
        if (raised != settings.UpdateMinimumBuild)
        {
            settings.UpdateMinimumBuild = raised;
            store.SaveSettings();
        }
        var result = VersionGate.Decide(ForkDefaults.BuildNumber, file, settings.UpdateMinimumBuild);
        log.LogInformation("Version gate: build {Build}, latest {Latest}, minimum {Minimum}: {Result}",
            ForkDefaults.BuildNumber, file?.Latest, settings.UpdateMinimumBuild, result);
        if (result == VersionGateResult.Required)
            Block(settings.UpdateMinimumBuild, file?.Version);
        return result;
    }

    private void Block(int minimum, string? version)
    {
        if (IsBlocked)
            return;
        IsBlocked = true;
        log.LogWarning("Build {Build} is below the minimum {Minimum}: blocking until updated", ForkDefaults.BuildNumber, minimum);
        try
        {
            StopBackgroundServices?.Invoke();
        }
        catch (Exception e)
        {
            log.LogWarning(e, "Could not stop background services before blocking");
        }
        if (Application.Current is not null)
            ShowDialog(version);
    }

    /// <summary>Hộp thoại không đóng được: chỉ "Tải bản mới" hoặc "Thoát".</summary>
    internal Window ShowDialog(string? version)
    {
        var message = version is null ? loc["update.blocked_message"] : loc.Format("update.blocked_message_version", version);
        var download = new Button { Content = loc["update.blocked_download"], IsDefault = true };
        download.Classes.Add("accent");
        var quit = new Button { Content = loc["update.blocked_quit"] };
        var allowClose = false;
        var dialog = new Window
        {
            Title = loc["update.blocked_title"],
            Width = 440,
            SizeToContent = SizeToContent.Height,
            CanResize = false,
            CanMinimize = false,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Topmost = true,
            ShowInTaskbar = true,
            Content = new StackPanel
            {
                Margin = new Thickness(24),
                Spacing = 18,
                Children =
                {
                    new TextBlock { Text = loc["update.blocked_title"], FontSize = 18, FontWeight = FontWeight.Bold, TextWrapping = TextWrapping.Wrap },
                    new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { quit, download },
                    },
                },
            },
        };
        // Không đóng được bằng nút X, Alt+F4 hay Esc: chỉ "Thoát" mới đóng (và thoát luôn ứng dụng).
        dialog.Closing += (_, e) =>
        {
            if (!allowClose)
                e.Cancel = true;
        };
        download.Click += (_, _) => OpenDownloadPage();
        quit.Click += (_, _) =>
        {
            allowClose = true;
            dialog.Close();
            Quit?.Invoke();
        };
        UiZoom.Track(dialog);
        _dialog = dialog;
        dialog.Show();
        dialog.Activate();
        return dialog;
    }

    /// <summary>Hộp thoại đang hiện (test).</summary>
    internal Window? Dialog => _dialog;

    private void OpenDownloadPage()
    {
        try
        {
            Process.Start(new ProcessStartInfo(VersionGate.DownloadPageUrl) { UseShellExecute = true });
        }
        catch (Exception e) when (e is Win32Exception or InvalidOperationException)
        {
            log.LogWarning(e, "Could not open {Url}", VersionGate.DownloadPageUrl);
        }
    }
}
