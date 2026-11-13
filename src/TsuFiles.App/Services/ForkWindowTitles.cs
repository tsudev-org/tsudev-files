// TsuFiles: tên hiển thị "TsuFiles" trên mọi tiêu đề cửa sổ, kể cả tiêu đề đặt sẵn trong .axaml (Title="Filee")
// và tiêu đề ghép lúc chạy, mà không phải sửa từng tệp giao diện của upstream. Cửa sổ donut đặt sẵn tiêu đề
// "TsuFiles Donut" (RadialWindow.axaml) - luật cửa sổ GNOME/KWin/Hyprland dựa vào đúng tiêu đề đó.

using Avalonia;
using Avalonia.Controls;
using TsuFiles.Core;

namespace TsuFiles.App.Services;

internal static class ForkWindowTitles
{
    private static int _registered;

    /// <summary>Đăng ký một lần cho cả tiến trình.</summary>
    public static void Register()
    {
        if (Interlocked.Exchange(ref _registered, 1) == 1)
            return;
        Window.TitleProperty.Changed.AddClassHandler<Window>((window, e) =>
        {
            if (e.NewValue is not string title)
                return;
            var branded = ForkDefaults.Brand(title);
            if (!string.Equals(branded, title, StringComparison.Ordinal))
                window.Title = branded;
        });
    }
}
