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

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Filee.slnx")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new DirectoryNotFoundException("Filee.slnx");
    }
}
