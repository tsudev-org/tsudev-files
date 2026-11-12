// TsuFiles: kiểu giao diện "TsuFiles Cú đêm" (ADR-0003). Dùng chung khung điều khiển của Soft (Styles/Soft.axaml), chỉ thay
// bề mặt và màu nhấn: đen tuyền + tím đo từ logo con cú (assets/brand/source/tsufiles-2.webp, sắc tím chiếm nhiều nhất
// là #8840F0), nâng/hạ độ sáng cho chữ màu nhấn đạt WCAG AA trên nền. Màu nhấn cố định theo thương hiệu, không theo màu
// người dùng chọn ở trang Giao diện.

using Avalonia.Controls;
using Avalonia.Media;

namespace TsuFiles.App.Services;

internal static class NightOwlPalette
{
    /// <summary>Tím con cú trên nền tối: cùng sắc với logo (#8840F0) nhưng sáng hơn để chữ màu nhấn đạt 4.5:1 trên #141414.</summary>
    public const string DarkAccent = "#A06BF5";

    /// <summary>Tím đậm hơn cho nền sáng: chữ trắng trên nút nhấn vẫn đạt WCAG AA.</summary>
    public const string LightAccent = "#6A2BD0";

    /// <summary>Nền trang tối: đen tuyền theo quyết định giao diện chung của hệ sinh thái (phiếu 20261110-05).</summary>
    public const string DarkPage = "#0A0A0A";

    private static readonly (string Key, string Light, string Dark)[] Surfaces =
    [
        ("PageBrush", "#F7F6FB", DarkPage),
        ("SurfaceBrush", "#FFFFFF", "#141414"),
        ("SurfaceAltBrush", "#EFECF7", "#1E1C24"),
        ("SubtleBorderBrush", "#1A2A1650", "#24FFFFFF"),
        ("TextPrimaryBrush", "#17151C", "#EDEDED"),
        ("TextSecondaryBrush", "#5F5A6B", "#A3A0AB"),
        ("DangerBrush", "#C73A39", "#F09595"),
        ("DangerFaintBrush", "#1AC73A39", "#26F09595"),
        ("SuccessBrush", "#0F7A5A", "#5DCAA5"),
        ("WarningBrush", "#9A5B0C", "#EF9F27"),
        ("ButtonBackground", "#FFFFFF", "#141414"),
        ("ButtonBackgroundPointerOver", "#EFECF7", "#1E1C24"),
        ("ButtonBackgroundPressed", "#E4DFF2", "#28252F"),
        ("ButtonBorderBrush", "#2E2A1650", "#38FFFFFF"),
        ("ButtonBorderBrushPointerOver", "#472A1650", "#52FFFFFF"),
        ("ButtonBorderBrushPressed", "#472A1650", "#52FFFFFF"),
    ];

    /// <summary>Màu nhấn của một biến thể.</summary>
    public static Color Accent(bool isDark) => Color.Parse(isDark ? DarkAccent : LightAccent);

    /// <summary>Ghi bề mặt + màu nhấn Cú đêm của một biến thể (sáng/tối) vào <paramref name="dictionary"/>.</summary>
    public static void Apply(IResourceDictionary dictionary, bool isDark)
    {
        foreach (var (key, light, dark) in Surfaces)
            dictionary[key] = new SolidColorBrush(Color.Parse(isDark ? dark : light));
        var accent = Accent(isDark);
        SoftPalette.ApplyAccent(dictionary, accent);
        // Chữ "màu nhấn đậm" (thẻ đang chọn, nhãn) trên nền tối phải SÁNG hơn màu nhấn, không tối hơn như ở Soft.
        if (isDark)
            dictionary["AccentStrongBrush"] = new SolidColorBrush(SoftPalette.Shade(accent, 0.25));
    }
}
