// TsuFiles: cấu hình trung tâm của bản fork Việt hóa.
// Mọi giá trị khác biệt so với repo gốc KnifeLemon/Filee đặt ở đây (hoặc Directory.Build.props cho giá trị lúc build),
// để khi gộp bản mới từ upstream chỉ cần soát một chỗ.

using System.Globalization;
using System.Text;

namespace TsuFiles.Core;

/// <summary>Giá trị mặc định riêng của bản fork TsuFiles.</summary>
public static class ForkDefaults
{
    /// <summary>
    /// Ngôn ngữ giao diện của bản cài mới: tiếng Việt. Người dùng vẫn đổi được trong Cài đặt
    /// (kể cả "auto" = theo ngôn ngữ hệ điều hành, hoặc "en").
    /// </summary>
    public const string DefaultLanguage = "vi";

    /// <summary>Ngôn ngữ dự phòng khi thiếu bản dịch một chuỗi: tiếng Anh (bản gốc, luôn đầy đủ).</summary>
    public const string FallbackLanguage = "en";

    /// <summary>Repo phát hành của bản fork (trình cập nhật đọc giá trị build TsuFilesRepositoryUrl; đây là bản sao để hiển thị).</summary>
    public const string RepositoryUrl = "https://github.com/tsudev-org/tsufiles";

    /// <summary>Nhà phát hành (ECOSYSTEM_IDENTITY mục 2.3): thương hiệu luôn viết thường.</summary>
    public const string PublisherName = "tsudev";

    /// <summary>Trang chủ hệ sinh thái tsudev; hộp Giới thiệu phải có liên kết mở được tới đây (ECOSYSTEM_IDENTITY 2.3).</summary>
    public const string HomepageUrl = "https://tsudev.com";

    /// <summary>Trang sản phẩm (thư mục website/ của repo).</summary>
    public const string ProductPageUrl = "https://files.tsudev.com";

    /// <summary>Trang Ủng hộ: nơi duy nhất liệt kê cách ủng hộ (website/src/data/ung-ho.json); nút "Tài trợ" của ứng dụng trỏ về đây.</summary>
    public const string SupportPageUrl = ProductPageUrl + "/ung-ho/";

    /// <summary>Tên hiển thị của ứng dụng (ADR-0003, 12/11/2026: "Filee" -> "tsudev-files" -> "TsuFiles").</summary>
    public const string DisplayName = "TsuFiles";

    /// <summary>
    /// Tên mà bản 2.4.0-vn.* đã cài và trình cập nhật của nó dựa vào: lối tắt (tsudev-files.lnk), script Nautilus,
    /// tiền tố tệp phát hành. Đổi sang "tsufiles" ở bản 2.4.1 kèm di trú (ADR-0003 bước T3), không đổi riêng lẻ.
    /// </summary>
    public const string InstalledName = "tsudev-files";

    /// <summary>
    /// Số build nguyên, tăng 1 ở MỖI bản phát hành (bắt buộc cập nhật so số này với "moiNhat"/"toiThieu" trong
    /// files.tsudev.com/phien-ban.json, xem Updates/VersionGate.cs và docs/vi/CAP-NHAT-BAT-BUOC.md).
    /// 1 = 2.4.0-vn.1 (bản đầu, chưa có cơ chế), 2 = bản phát hành kế tiếp (2.4.1).
    /// </summary>
    public const int BuildNumber = 2;

    /// <summary>Tên của dự án gốc; còn là tên tệp chạy và khóa registry tới khi T3b đổi xong (ADR-0003).</summary>
    public const string UpstreamName = "Filee";

    /// <summary>
    /// Tên thư mục dữ liệu từ bản 2.4.1 (ADR-0003 T3b): %APPDATA%\TsuFiles (cài đặt, nhật ký), %LOCALAPPDATA%\TsuFiles
    /// (engine đã tải), %TEMP%\TsuFiles và ~/Library/Caches/TsuFiles (tệp làm việc). Bản cũ dùng
    /// <see cref="LegacyDataFolderName"/>; Settings/LegacyDataMigration.cs chuyển sang một lần.
    /// </summary>
    public const string DataFolderName = "TsuFiles";

    /// <summary>Tên thư mục dữ liệu của bản 2.4.0-vn.* và Filee gốc.</summary>
    public const string LegacyDataFolderName = UpstreamName;

    /// <summary>Tên thư mục chữ thường kiểu Unix: ~/.cache/tsufiles (bản cũ: ~/.cache/filee).</summary>
    public const string UnixFolderName = "tsufiles";

    /// <summary>Tiền tố tên tệp phát hành: tsudev-files-&lt;phiên bản&gt;-win-Setup.exe, ...</summary>
    public const string ReleaseFilePrefix = InstalledName;

    /// <summary>
    /// Đổi tên hiển thị trong một đoạn văn bản cho người dùng (bản dịch, tiêu đề cửa sổ, thông báo dòng lệnh).
    /// Chỉ thay chữ "Filee" đứng riêng; giữ nguyên các định danh: tên tệp (Filee.exe, Filee.app), đường dẫn
    /// (%APPDATA%\Filee, KnifeLemon/Filee), "Filee Donut" (tiêu đề cửa sổ mà luật KWin/Hyprland dựa vào) và chữ
    /// thường "filee" (lệnh dòng lệnh). Ghép từ kiểu tiếng Đức "Filee-Erweiterung" vẫn được đổi.
    /// </summary>
    /// <remarks>
    /// Bộ quét ký tự một lượt, O(n), không cấp phát khi không có gì để đổi, không bao giờ ném ngoại lệ. Trước đây là
    /// regex có giới hạn 100 ms: khi máy bận (khởi động lạnh, JIT) đồng hồ vượt giới hạn dù mẫu tuyến tính, và
    /// RegexMatchTimeoutException làm sập ứng dụng, Setup hay dòng lệnh (`filee help engines`). Quy tắc giữ đúng mẫu cũ
    /// <c>(?&lt;![A-Za-z0-9_.\\/%@~-])Filee(?![A-Za-z0-9_\\/@]|\.(?:exe|app|lnk|sh|cmd|json|txt)\b| Donut)</c>;
    /// ForkBrandingTests so kết quả với chính mẫu đó trên mọi bản dịch và chuỗi ngẫu nhiên.
    /// </remarks>
    public static string Brand(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        var at = text.IndexOf(UpstreamName, StringComparison.Ordinal);
        if (at < 0) return text;

        StringBuilder? result = null;
        var copied = 0;
        for (; at >= 0; at = text.IndexOf(UpstreamName, at + 1, StringComparison.Ordinal))
        {
            if (!IsDisplayWord(text, at)) continue;
            result ??= new StringBuilder(text.Length + 16);
            result.Append(text, copied, at - copied).Append(DisplayName);
            copied = at + UpstreamName.Length;
            at = copied - 1;   // "Filee" không tự chồng lên chính nó; quét tiếp sau chữ vừa thay
        }
        if (result is null) return text;
        return result.Append(text, copied, text.Length - copied).ToString();
    }

    private static readonly string[] IdentifierExtensions = ["exe", "app", "lnk", "sh", "cmd", "json", "txt"];

    /// <summary>"Filee" tại <paramref name="at"/> là chữ hiển thị (đổi được), không phải một phần của định danh.</summary>
    private static bool IsDisplayWord(string text, int at)
    {
        // Đứng ngay sau chữ/số ASCII, "_", ".", "\", "/", "%", "@", "~", "-": thuộc định danh hoặc đường dẫn.
        if (at > 0)
        {
            var before = text[at - 1];
            if (char.IsAsciiLetterOrDigit(before) || before is '_' or '.' or '\\' or '/' or '%' or '@' or '~' or '-')
                return false;
        }
        var after = at + UpstreamName.Length;
        if (after >= text.Length) return true;
        var next = text[after];
        if (char.IsAsciiLetterOrDigit(next) || next is '_' or '\\' or '/' or '@') return false;
        if (next == ' ' && string.CompareOrdinal(text, after, " Donut", 0, 6) == 0) return false;
        if (next == '.')
        {
            foreach (var ext in IdentifierExtensions)
            {
                var end = after + 1 + ext.Length;
                if (end <= text.Length && string.CompareOrdinal(text, after + 1, ext, 0, ext.Length) == 0
                    && (end == text.Length || !IsRegexWordChar(text[end])))
                    return false;
            }
        }
        return true;
    }

    /// <summary>Ký tự "chữ" khi .NET xét ranh giới \b: chữ cái (L), dấu kết hợp (Mn), chữ số (Nd), nối (Pc), ZWNJ, ZWJ.</summary>
    private static bool IsRegexWordChar(char c) => c is '\u200C' or '\u200D' || CharUnicodeInfo.GetUnicodeCategory(c) switch
    {
        UnicodeCategory.UppercaseLetter or UnicodeCategory.LowercaseLetter or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter or UnicodeCategory.OtherLetter or UnicodeCategory.NonSpacingMark
            or UnicodeCategory.DecimalDigitNumber or UnicodeCategory.ConnectorPunctuation => true,
        _ => false,
    };
}
