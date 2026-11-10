// tsudev-files: tên hiển thị "tsudev-files" thay "Filee" trong văn bản cho người dùng, giữ nguyên các định danh.

using Filee.Core;

namespace Filee.Core.Tests;

public sealed class ForkBrandingTests
{
    [Theory]
    [InlineData("Filee", "tsudev-files")]
    [InlineData("Filee is running.", "tsudev-files is running.")]
    [InlineData("Thank you for using Filee.", "Thank you for using tsudev-files.")]
    [InlineData("Chuyển đổi bằng Filee", "Chuyển đổi bằng tsudev-files")]
    [InlineData("Filee's files", "tsudev-files's files")]
    [InlineData("Die Filee-Erweiterung", "Die tsudev-files-Erweiterung")]
    [InlineData("Filee는 무료입니다", "tsudev-files는 무료입니다")]
    [InlineData("使用Filee转换", "使用tsudev-files转换")]
    [InlineData("“Filee donut at the cursor”", "“tsudev-files donut at the cursor”")]
    [InlineData("\nFilee downloads", "\ntsudev-files downloads")]
    public void Display_text_gets_the_fork_name(string text, string expected) =>
        Assert.Equal(expected, ForkDefaults.Brand(text));

    [Theory]
    [InlineData("Filee.exe")]
    [InlineData("Filee.app")]
    [InlineData(@"%APPDATA%\Filee")]
    [InlineData("~/.config/Filee")]
    [InlineData("https://github.com/KnifeLemon/Filee")]
    [InlineData("Filee Donut")]
    [InlineData("filee convert a.png")]
    [InlineData("FileeExpectedPublisher")]
    [InlineData("com.filee.app")]
    [InlineData("")]
    public void Identifiers_stay(string text) => Assert.Equal(text, ForkDefaults.Brand(text));

    // Mẫu regex cũ của Brand (trước khi thay bằng bộ quét ký tự), không giới hạn thời gian: dùng làm chuẩn đối chiếu.
    private static readonly System.Text.RegularExpressions.Regex OldPattern = new(
        @"(?<![A-Za-z0-9_.\\/%@~-])Filee(?![A-Za-z0-9_\\/@]|\.(?:exe|app|lnk|sh|cmd|json|txt)\b| Donut)",
        System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    private static string Old(string text) => OldPattern.Replace(text, ForkDefaults.DisplayName);

    [Theory]
    [InlineData("Filee.exe.")]
    [InlineData("Filee.exes")]
    [InlineData("Filee.ex")]
    [InlineData("Filee.EXE")]
    [InlineData("Filee.json:")]
    [InlineData("Filee.txtà")]
    [InlineData("Filee.sh‍")]
    [InlineData("Filee.cmd_")]
    [InlineData("Filee Donuts")]
    [InlineData("Filee  Donut")]
    [InlineData("FileeFilee Filee")]
    [InlineData("-Filee Filee-")]
    [InlineData("éFileeé")]
    [InlineData("Fileé")]
    [InlineData("Filee.")]
    [InlineData("Filee.apṕ")]
    [InlineData("(Filee)[Filee]{Filee}")]
    [InlineData("Filee\tDonut")]
    public void Edge_cases_match_the_old_pattern(string text) => Assert.Equal(Old(text), ForkDefaults.Brand(text));

    [Fact]
    public void Random_text_matches_the_old_pattern()
    {
        // Bảng chữ dồn vào các ký tự quyết định: biên trái/phải, đuôi tệp, " Donut", chữ Unicode, dấu kết hợp, ZWJ.
        string[] pieces = ["Filee", "Filee", "Filee", ".", "exe", "app", "lnk", "sh", "cmd", "json", "txt", " Donut", " ",
            "\\", "/", "%", "@", "~", "-", "_", "a", "Z", "0", "é", "́", "‍", "‌", "한", "转", "ß", "\n", ":",
            "E", "x", "D", "onut", "F", "ilee", "Ⅰ", "²", "ः"];
        var random = new Random(20261110);
        for (var i = 0; i < 50_000; i++)
        {
            var sb = new System.Text.StringBuilder();
            for (var n = random.Next(1, 12); n > 0; n--) sb.Append(pieces[random.Next(pieces.Length)]);
            var text = sb.ToString();
            Assert.True(Old(text) == ForkDefaults.Brand(text), $"Khác mẫu cũ: \"{text}\" -> \"{ForkDefaults.Brand(text)}\", mẫu cũ \"{Old(text)}\"");
        }
    }

    [Fact]
    public void Every_translation_matches_the_old_pattern()
    {
        foreach (var folder in new[] { "src/Filee.App/Assets/i18n", "src/Filee.Cli", "src/Filee.Setup" })
        foreach (var file in Directory.GetFiles(Path.Combine(RepoRoot(), folder), "*.json", SearchOption.AllDirectories))
        {
            using var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(file));
            if (json.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object) continue;
            foreach (var property in json.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind != System.Text.Json.JsonValueKind.String) continue;
                var text = property.Value.GetString()!;
                Assert.True(Old(text) == ForkDefaults.Brand(text), $"{Path.GetFileName(file)}: {property.Name}");
            }
        }
    }

    // Lỗi gốc: regex giới hạn 100 ms ném RegexMatchTimeoutException khi máy bận. Bộ quét không có đồng hồ: dù mọi lõi
    // đều bận và chuỗi rất dài, Brand vẫn trả kết quả, không ném ngoại lệ.
    [Fact]
    public void Brand_never_throws_under_load()
    {
        var huge = string.Concat(Enumerable.Repeat("Filee.exe Filee/ Filee Donut Filee ", 200_000));
        var expected = huge.Replace("Filee ", "tsudev-files ", StringComparison.Ordinal)
            .Replace("tsudev-files Donut", "Filee Donut", StringComparison.Ordinal);
        var results = new string[Environment.ProcessorCount * 2];
        Parallel.For(0, results.Length, new ParallelOptions { MaxDegreeOfParallelism = results.Length },
            i => results[i] = ForkDefaults.Brand(huge));
        Assert.All(results, r => Assert.Equal(expected, r));
    }

    [Fact]
    public void Text_without_the_name_is_returned_as_is()
    {
        const string text = "Chuyển đổi tệp ngoại tuyến";
        Assert.Same(text, ForkDefaults.Brand(text));
        const string identifiers = "Filee.exe và %APPDATA%\\Filee";
        Assert.Same(identifiers, ForkDefaults.Brand(identifiers));
    }

    [Fact]
    public void Branding_twice_changes_nothing() =>
        Assert.Equal(ForkDefaults.Brand("Filee is free"), ForkDefaults.Brand(ForkDefaults.Brand("Filee is free")));

    [Fact]
    public void Every_translation_names_the_fork()
    {
        var folder = Path.Combine(RepoRoot(), "src", "Filee.App", "Assets", "i18n");
        foreach (var file in Directory.GetFiles(folder, "*.json"))
        {
            using var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(file));
            foreach (var property in json.RootElement.EnumerateObject())
            {
                var text = ForkDefaults.Brand(property.Value.GetString() ?? "");
                // Sau khi đổi tên, "Filee" chỉ còn trong định danh (tệp chạy, đường dẫn, tiêu đề cửa sổ donut).
                var rest = System.Text.RegularExpressions.Regex.Replace(text,
                    @"Filee(\.(exe|app)|\\|/| Donut)|[\\/]Filee|%APPDATA%\\Filee", "");
                Assert.False(rest.Contains("Filee", StringComparison.Ordinal), $"{Path.GetFileName(file)}: {property.Name}: {text}");
            }
        }
    }

    // Chữ viết cứng trong giao diện không đi qua bản dịch (thanh bên, trang Giới thiệu, màn chào của Setup):
    // phải dùng {x:Static fork:ForkDefaults.DisplayName}. Tiêu đề cửa sổ (Title=) đã có ForkWindowTitles lo.
    [Fact]
    public void No_view_shows_the_upstream_name_as_text()
    {
        var src = Path.Combine(RepoRoot(), "src");
        var pattern = new System.Text.RegularExpressions.Regex(@"\b(Text|Content|Header|ToolTip\.Tip)=""Filee""");
        foreach (var file in Directory.GetFiles(src, "*.axaml", SearchOption.AllDirectories))
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
                Assert.False(pattern.IsMatch(lines[i]), $"{Path.GetRelativePath(src, file)}:{i + 1}: {lines[i].Trim()}");
        }
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Filee.slnx")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new DirectoryNotFoundException("Filee.slnx");
    }
}
