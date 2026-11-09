// tsudev-files: cấu hình trung tâm của bản fork Việt hóa.
// Mọi giá trị khác biệt so với repo gốc KnifeLemon/Filee đặt ở đây (hoặc Directory.Build.props cho giá trị lúc build),
// để khi gộp bản mới từ upstream chỉ cần soát một chỗ.

using System.Text.RegularExpressions;

namespace Filee.Core;

/// <summary>Giá trị mặc định riêng của bản fork tsudev-files.</summary>
public static partial class ForkDefaults
{
    /// <summary>
    /// Ngôn ngữ giao diện của bản cài mới: tiếng Việt. Người dùng vẫn đổi được trong Cài đặt
    /// (kể cả "auto" = theo ngôn ngữ hệ điều hành, hoặc "en").
    /// </summary>
    public const string DefaultLanguage = "vi";

    /// <summary>Ngôn ngữ dự phòng khi thiếu bản dịch một chuỗi: tiếng Anh (bản gốc, luôn đầy đủ).</summary>
    public const string FallbackLanguage = "en";

    /// <summary>Repo phát hành của bản fork (trình cập nhật đọc giá trị build FileeRepositoryUrl; đây là bản sao để hiển thị).</summary>
    public const string RepositoryUrl = "https://github.com/tsudev-tsudev/tsudev-files";

    /// <summary>Tên hiển thị của ứng dụng (quyết định 09/11/2026: "Filee" -> "tsudev-files").</summary>
    public const string DisplayName = "tsudev-files";

    /// <summary>Tên của dự án gốc; vẫn là tên namespace, tệp chạy, thư mục dữ liệu và khóa registry.</summary>
    public const string UpstreamName = "Filee";

    /// <summary>Tiền tố tên tệp phát hành: tsudev-files-&lt;phiên bản&gt;-win-Setup.exe, ...</summary>
    public const string ReleaseFilePrefix = DisplayName;

    /// <summary>
    /// Đổi tên hiển thị trong một đoạn văn bản cho người dùng (bản dịch, tiêu đề cửa sổ, thông báo dòng lệnh).
    /// Chỉ thay chữ "Filee" đứng riêng; giữ nguyên các định danh: tên tệp (Filee.exe, Filee.app), đường dẫn
    /// (%APPDATA%\Filee, KnifeLemon/Filee), "Filee Donut" (tiêu đề cửa sổ mà luật KWin/Hyprland dựa vào) và chữ
    /// thường "filee" (lệnh dòng lệnh). Ghép từ kiểu tiếng Đức "Filee-Erweiterung" vẫn được đổi.
    /// </summary>
    public static string Brand(string text) =>
        string.IsNullOrEmpty(text) || !text.Contains(UpstreamName, StringComparison.Ordinal)
            ? text
            : UpstreamWord().Replace(text, DisplayName);

    [GeneratedRegex(@"(?<![A-Za-z0-9_.\\/%@~-])Filee(?![A-Za-z0-9_\\/@]|\.(?:exe|app|lnk|sh|cmd|json|txt)\b| Donut)",
        RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex UpstreamWord();
}
