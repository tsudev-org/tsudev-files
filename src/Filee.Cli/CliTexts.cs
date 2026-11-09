// tsudev-files: văn bản của dòng lệnh, đọc từ file dịch của ứng dụng (nhúng sẵn): thông báo lỗi của từng file và tên
// các preset có sẵn. Mặc định tiếng Việt (vi.json), thiếu chuỗi nào thì lấy tiếng Anh (en.json).
// Chọn ngôn ngữ bằng biến môi trường FILEE_LANG=vi|en (xem CliLanguage).

using System.Globalization;
using System.Text.Json;
using Filee.Core;
using Filee.Core.Localization;

namespace Filee.Cli;

/// <summary>Ngôn ngữ của dòng lệnh: "vi" (mặc định) hoặc "en".</summary>
internal static class CliLanguage
{
    /// <summary>Biến môi trường chọn ngôn ngữ dòng lệnh.</summary>
    public const string EnvironmentVariable = "FILEE_LANG";

    /// <summary>Ngôn ngữ đang dùng. Chỉ nhận danh sách trắng "vi" | "en"; giá trị lạ quay về mặc định.</summary>
    public static string Current { get; set; } = FromEnvironment(Environment.GetEnvironmentVariable(EnvironmentVariable));

    public static bool IsVietnamese => Current == "vi";

    internal static string FromEnvironment(string? value) =>
        value?.Trim().ToLowerInvariant() switch
        {
            "en" or "en-us" or "english" => "en",
            "vi" or "vi-vn" or "vietnamese" => "vi",
            _ => ForkDefaults.DefaultLanguage,
        };

    /// <summary>Chọn câu theo ngôn ngữ đang dùng.</summary>
    public static string T(string vietnamese, string english) => ForkDefaults.Brand(IsVietnamese ? vietnamese : english);
}

internal sealed class CliTexts : ILocalizer
{
    private readonly Dictionary<string, string> _texts;
    private readonly Dictionary<string, string> _fallback;

    public CliTexts() : this(CliLanguage.Current) { }

    public CliTexts(string language)
    {
        Language = language == "vi" ? "vi" : "en";
        _fallback = Load("en");
        _texts = Language == "en" ? _fallback : Load(Language);
    }

    private static Dictionary<string, string> Load(string code)
    {
        using var stream = typeof(CliTexts).Assembly.GetManifestResourceStream($"Filee.Cli.{code}.json")
                           ?? throw new InvalidOperationException($"Embedded {code}.json is missing.");
        using var json = JsonDocument.Parse(stream);
        return json.RootElement.EnumerateObject()
            .Where(p => p.Value.ValueKind == JsonValueKind.String)
            .ToDictionary(p => p.Name, p => ForkDefaults.Brand(p.Value.GetString()!));
    }

    public string Language { get; }

    public string this[string key] =>
        _texts.TryGetValue(key, out var text) || _fallback.TryGetValue(key, out text) ? text : key;

    public string Format(string key, params object[] args) => string.Format(CultureInfo.InvariantCulture, this[key], args);

    public event EventHandler? LanguageChanged { add { } remove { } }
}
