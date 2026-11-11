// tsudev-files: kiểm thử phần cập nhật riêng của bản fork.
//  - Chữ ký do tools/sign-checksums-vn.sh (openssl, Linux/WSL) tạo ra phải được UpdateSignature chấp nhận.
//  - Bản build phải mang khóa công khai và repo của fork, không phải của upstream (nếu không, trình cập nhật sẽ cài
//    bản phát hành của repo gốc, hoặc từ chối mọi bản của fork).
// Khóa dùng ở đây là khóa THỬ, tạo riêng cho test và đã hủy; không liên quan khóa phát hành thật.

using System.Security.Cryptography;
using System.Text;
using Filee.App.Services;
using Filee.Core;

namespace Filee.App.Tests;

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
        // Khóa chỉ được nhúng vào bản phát hành (-p:FileeRelease=true, Filee.App.csproj), nên kiểm tại nguồn của nó.
        var props = System.Xml.Linq.XDocument.Load(Path.Combine(FindRepoRoot(), "Directory.Build.props"));
        string? Value(string name) => props.Descendants(name).FirstOrDefault()?.Value.Trim();

        var key = Value("FileeUpdatePublicKey");
        Assert.False(string.IsNullOrWhiteSpace(key));
        var ring = UpdateSignature.Keys(key);
        // Khóa chính + ít nhất một khóa dự phòng ngoại tuyến, khác nhau, không có khóa upstream.
        Assert.InRange(ring.Length, 2, UpdateSignature.MaxKeys);
        Assert.Equal(ring.Length, ring.Distinct().Count());
        Assert.DoesNotContain(UpstreamPublicKey, ring);
        foreach (var k in ring)
        {
            using var ec = ECDsa.Create();
            ec.ImportSubjectPublicKeyInfo(Convert.FromBase64String(k), out _);
            Assert.Equal(ECCurve.NamedCurves.nistP256.Oid.Value, ec.ExportParameters(false).Curve.Oid.Value);
        }
        Assert.True(string.IsNullOrEmpty(Value("FileeExpectedPublisher")), "chưa có chứng thư riêng thì không ghim nhà phát hành");
        Assert.Equal(ForkDefaults.RepositoryUrl, Value("FileeRepositoryUrl"));
        Assert.Equal(ForkDefaults.RepositoryUrl, UpdateService.RepositoryUrl);
        Assert.StartsWith(ForkDefaults.RepositoryUrl, UpdateService.WebsiteUrl);
    }

    [Fact]
    public void Key_ring_accepts_a_signature_of_any_of_its_keys()
    {
        using var primary = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        using var backup = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        using var stranger = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var data = "abc  tsudev-files-9.9.9-win-Setup.exe\n"u8.ToArray();
        var ring = $"{Pub(primary)} | {Pub(backup)}";

        Assert.True(UpdateSignature.Verify(data, Sign(primary, data), ring));
        Assert.True(UpdateSignature.Verify(data, Sign(backup, data), ring));
        Assert.False(UpdateSignature.Verify(data, Sign(stranger, data), ring));
        // Một khóa hỏng trong vòng không làm hỏng các khóa còn lại.
        Assert.True(UpdateSignature.Verify(data, Sign(backup, data), $"AAAA|{Pub(backup)}"));
        // Quá nhiều khóa: nghi bị chèn, từ chối hết.
        var tooMany = string.Join('|', Enumerable.Repeat(Pub(primary), UpdateSignature.MaxKeys + 1));
        Assert.False(UpdateSignature.Verify(data, Sign(primary, data), tooMany));
        Assert.False(UpdateSignature.Verify(data, Sign(primary, data), ""));
    }

    [Fact]
    public void Key_with_trailing_bytes_is_refused()
    {
        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var data = "x"u8.ToArray();
        var padded = Convert.ToBase64String([.. key.ExportSubjectPublicKeyInfo(), 0, 0]);
        Assert.False(UpdateSignature.Verify(data, Sign(key, data), padded));
    }

    private static string Pub(ECDsa key) => Convert.ToBase64String(key.ExportSubjectPublicKeyInfo());

    private static string Sign(ECDsa key, byte[] data) =>
        Convert.ToBase64String(key.SignData(data, HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation));

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Filee.slnx")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new DirectoryNotFoundException("Filee.slnx");
    }
}
