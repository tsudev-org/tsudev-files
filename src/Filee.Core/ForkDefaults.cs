// tsudev-files: cấu hình trung tâm của bản fork Việt hóa.
// Mọi giá trị khác biệt so với repo gốc KnifeLemon/Filee đặt ở đây (hoặc Directory.Build.props cho giá trị lúc build),
// để khi gộp bản mới từ upstream chỉ cần soát một chỗ.

namespace Filee.Core;

/// <summary>Giá trị mặc định riêng của bản fork tsudev-files.</summary>
public static class ForkDefaults
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
}
