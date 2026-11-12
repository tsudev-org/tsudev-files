// TsuFiles: hộp thoại bắt buộc cập nhật (ForcedUpdateService) - chặn theo mức tối thiểu đã lưu hay vừa tải, không đóng
// được, dừng dịch vụ nền, chỉ "Thoát" mới đóng.

using System.Net;
using System.Text;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.LogicalTree;
using Microsoft.Extensions.Logging.Abstractions;
using TsuFiles.App.Services;
using TsuFiles.Core;
using TsuFiles.Core.Localization;
using TsuFiles.Core.Settings;
using TsuFiles.Core.Updates;

namespace TsuFiles.App.Tests;

public sealed class ForcedUpdateTests
{
    private sealed class Stub(string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Encoding.UTF8.GetBytes(body)) });
    }

    private static string File(int latest, int minimum) =>
        $$"""{"schema":1,"phienBan":"9.9.9","moiNhat":{{latest}},"toiThieu":{{minimum}}}""";

    private static (ForcedUpdateService Gate, UserDataStore Store, string Directory) Make()
    {
        TestServices.EnsureInitialized("vi");
        var directory = System.IO.Directory.CreateTempSubdirectory("tsufiles-gate-").FullName;
        var store = new UserDataStore(directory);
        store.Load();
        return (new ForcedUpdateService(store, AppHost.Get<ILocalizer>(), NullLogger<ForcedUpdateService>.Instance), store, directory);
    }

    [AvaloniaFact]
    public async Task A_current_build_is_not_blocked_and_nothing_is_shown()
    {
        var (gate, store, directory) = Make();
        try
        {
            var result = await gate.CheckAsync(new Stub(File(ForkDefaults.BuildNumber, 1)),
                cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(VersionGateResult.UpToDate, result);
            Assert.False(gate.IsBlocked);
            Assert.Null(gate.Dialog);
            Assert.Equal(1, store.Settings.UpdateMinimumBuild);
        }
        finally
        {
            System.IO.Directory.Delete(directory, true);
        }
    }

    [AvaloniaFact]
    public async Task A_build_below_the_minimum_is_blocked_by_a_dialog_that_only_quit_closes()
    {
        var (gate, store, directory) = Make();
        var stopped = 0;
        var quit = 0;
        gate.StopBackgroundServices = () => stopped++;
        gate.Quit = () => quit++;
        try
        {
            var result = await gate.CheckAsync(new Stub(File(ForkDefaults.BuildNumber + 2, ForkDefaults.BuildNumber + 1)),
                cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(VersionGateResult.Required, result);
            Assert.True(gate.IsBlocked);
            Assert.Equal(1, stopped);
            Assert.Equal(ForkDefaults.BuildNumber + 1, store.Settings.UpdateMinimumBuild);

            var dialog = Assert.IsAssignableFrom<Window>(gate.Dialog);
            Assert.Equal("Cần cập nhật", dialog.Title);
            dialog.Close();
            Assert.True(dialog.IsVisible); // X/Alt+F4 bị từ chối

            var buttons = dialog.GetLogicalDescendants().OfType<Button>().ToList();
            Assert.Equal(["Thoát", "Tải bản mới"], buttons.Select(b => b.Content as string));
            buttons[0].RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
            Assert.False(dialog.IsVisible);
            Assert.Equal(1, quit);
        }
        finally
        {
            gate.Dialog?.Close();
            System.IO.Directory.Delete(directory, true);
        }
    }

    [AvaloniaFact]
    public async Task Offline_or_with_update_checks_off_the_stored_minimum_still_blocks()
    {
        var (gate, store, directory) = Make();
        store.Settings.CheckForUpdates = false;
        store.Settings.UpdateMinimumBuild = ForkDefaults.BuildNumber + 1;
        gate.Quit = () => { };
        try
        {
            Assert.Equal(VersionGateResult.Required, await gate.CheckAsync(new Stub("hỏng"), cancellationToken: TestContext.Current.CancellationToken));
            Assert.True(gate.IsBlocked);
        }
        finally
        {
            if (gate.Dialog is { } dialog)
                dialog.GetLogicalDescendants().OfType<Button>().First().RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
            System.IO.Directory.Delete(directory, true);
        }
    }

    [AvaloniaFact]
    public async Task A_file_lowered_later_never_lowers_the_stored_minimum()
    {
        var (gate, store, directory) = Make();
        store.Settings.UpdateMinimumBuild = ForkDefaults.BuildNumber;
        try
        {
            var result = await gate.CheckAsync(new Stub(File(ForkDefaults.BuildNumber, 1)),
                cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(VersionGateResult.UpToDate, result);
            Assert.Equal(ForkDefaults.BuildNumber, store.Settings.UpdateMinimumBuild);
        }
        finally
        {
            System.IO.Directory.Delete(directory, true);
        }
    }
}
