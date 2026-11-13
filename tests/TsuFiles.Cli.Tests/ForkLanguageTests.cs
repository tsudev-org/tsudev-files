// TsuFiles: kiểm thử ngôn ngữ dòng lệnh của bản fork. Các test gốc của upstream kiểm câu tiếng Anh, nên module
// initializer đặt TSUFILES_LANG=en cho cả assembly test; các test dưới đây tự chuyển sang tiếng Việt khi cần.

using System.Runtime.CompilerServices;

namespace TsuFiles.Cli.Tests;

internal static class CliTestLanguage
{
    [ModuleInitializer]
    internal static void UseEnglishForUpstreamTests() => CliLanguage.Current = "en";
}

public sealed class ForkLanguageTests
{
    [Theory]
    [InlineData(null, "vi")]
    [InlineData("", "vi")]
    [InlineData("vi", "vi")]
    [InlineData("VI-vn", "vi")]
    [InlineData("en", "en")]
    [InlineData("English", "en")]
    [InlineData("ko", "vi")]           // ngoài danh sách trắng → mặc định
    [InlineData("en;rm -rf /", "vi")]  // giá trị lạ không bao giờ được dùng nguyên văn
    public void Language_from_environment_is_whitelisted(string? value, string expected) =>
        Assert.Equal(expected, CliLanguage.FromEnvironment(value));

    [Theory]
    [InlineData("vi")]
    [InlineData("en")]
    public void Help_names_the_fork_and_credits_the_original(string language)
    {
        var text = Help.Text(null, language);
        Assert.StartsWith("TsuFiles (", text.TrimStart());
        Assert.Contains("KnifeLemon/Filee", text);
        Assert.Contains("tsufiles ", text); // ADR-0003: lệnh đổi thành tsufiles, không bí danh filee
        Assert.DoesNotContain("filee ", text);
        Assert.DoesNotMatch(@"(?<![\\/.])\bFilee\b(?![./\\])", text.Replace("KnifeLemon/Filee", ""));
    }

    [Fact]
    public void Vietnamese_texts_fall_back_to_english()
    {
        var vi = new CliTexts("vi");
        var en = new CliTexts("en");
        Assert.Equal("vi", vi.Language);
        Assert.NotEqual(en["error.no_route"], vi["error.no_route"]);
        Assert.Equal("no.such.key", vi["no.such.key"]);
    }

    [Fact]
    public void Help_is_vietnamese_by_default_and_english_on_request()
    {
        Assert.Contains("Cách dùng", Help.Text(null, "vi"));
        Assert.Contains("Cách dùng", Help.Text("convert", "vi"));
        Assert.Contains("Cách dùng", Help.Text("watch", "vi"));
        Assert.Contains("TSUFILES_LANG", Help.Text(null, "vi"));
        Assert.Contains("Usage", Help.Text(null, "en"));
    }
}
