// TsuFiles: kiểm thử phần cập nhật riêng của bản fork.
//  - Chữ ký do tools/sign-checksums-vn.sh (openssl, Linux/WSL) tạo ra phải được UpdateSignature chấp nhận.
//  - Bản build phải mang khóa công khai và repo của fork, không phải của upstream (nếu không, trình cập nhật sẽ cài
//    bản phát hành của repo gốc, hoặc từ chối mọi bản của fork).
// Khóa dùng ở đây là khóa THỬ, tạo riêng cho test và đã hủy; không liên quan khóa phát hành thật.

using System.Text;
using TsuFiles.App.Services;
using TsuFiles.Core;

namespace TsuFiles.App.Tests;

public sealed class ForkUpdateTests
{
    private const string TestPublicKey = "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEfywnH6/4hnH1zBnTh9UeNhT4BjvJgE3k9zs+AojRGZUL5Lb8ijetCBjX6JQl1HGY+DAtW+rx7zak6dneS5hukA==";
    private const string OpensslSignature = "peh4yeuldn7LIHjuD8Lpzo76kGfHL+fs0f49RAsE+Jkz5S+kkcR3WgvXcAc6PqDH7P7OHoVIcdddI8fBUk5chw==";
    private const string SignedText = "abc123  Filee-9.9.9-win-Setup.exe\n";
    private const string UpstreamPublicKey =
        "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE8mlW6UZaGmh9cKDhL25Bfl7SJscOpTzRvHrfZGsJOOOIP7zH58g/XyM59H3sDeV4QQRul3TeKqg2x6In1S+uUA==";

    [Fact]
    public void Signature_made_with_openssl_tool_verifies()
    {
        Assert.True(UpdateSignature.Verify(Encoding.ASCII.GetBytes(SignedText), OpensslSignature, TestPublicKey));
    }

    [Fact]
    public void Tampered_checksum_file_is_refused()
    {
        var tampered = SignedText.Replace("abc123", "abc124");
        Assert.False(UpdateSignature.Verify(Encoding.ASCII.GetBytes(tampered), OpensslSignature, TestPublicKey));
        Assert.False(UpdateSignature.Verify(Encoding.ASCII.GetBytes(SignedText), OpensslSignature, UpstreamPublicKey));
    }

    [Fact]
    public void Release_build_settings_carry_the_fork_key_and_repository()
    {
        // Khóa chỉ được nhúng vào bản phát hành (-p:TsuFilesRelease=true, TsuFiles.App.csproj), nên kiểm tại nguồn của nó.
        var props = System.Xml.Linq.XDocument.Load(Path.Combine(FindRepoRoot(), "Directory.Build.props"));
        string? Value(string name) => props.Descendants(name).FirstOrDefault()?.Value.Trim();

        var key = Value("TsuFilesUpdatePublicKey");
        Assert.False(string.IsNullOrWhiteSpace(key));
        Assert.NotEqual(UpstreamPublicKey, key);
        Assert.True(string.IsNullOrEmpty(Value("TsuFilesExpectedPublisher")), "chưa có chứng thư riêng thì không ghim nhà phát hành");
        Assert.Equal(ForkDefaults.RepositoryUrl, Value("TsuFilesRepositoryUrl"));
        Assert.Equal(ForkDefaults.RepositoryUrl, UpdateService.RepositoryUrl);
        Assert.StartsWith(ForkDefaults.RepositoryUrl, UpdateService.WebsiteUrl);
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "TsuFiles.slnx")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new DirectoryNotFoundException("TsuFiles.slnx");
    }
}
