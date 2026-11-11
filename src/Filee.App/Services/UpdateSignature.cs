// Signatures over a release's SHA256SUMS files. The checksum file only shows that a download arrived intact: it sits on
// the same release as the installer, so whoever could replace one could replace the other. Each checksum file is
// therefore signed with Filee's update key (ECDSA P-256 over SHA-256) on the maintainer's PC
// (build/sign-checksums.ps1, run by build/sign-release.ps1), and the signature is attached to the release next to it
// as <name>.sig. Official builds carry the public half (FileeUpdatePublicKey in Directory.Build.props), so the updater
// can tell a checksum file Filee wrote from any other, on every platform.

using System.Security.Cryptography;

namespace Filee.App.Services;

internal static class UpdateSignature
{
    /// <summary>The signature of "Filee-2.4.0-SHA256SUMS.txt" is "Filee-2.4.0-SHA256SUMS.txt.sig".</summary>
    public const string Extension = ".sig";

    /// <summary>
    /// True when <paramref name="signature"/> (base64 of the 64-byte r‖s form) is a signature of
    /// <paramref name="data"/> by <paramref name="publicKey"/> (base64 SubjectPublicKeyInfo of a P-256 key). A key or
    /// signature that can't be read counts as a wrong one.
    /// </summary>
    /// <summary>Most keys a build may trust at once (primary + offline backup, with room for one rotation).</summary>
    // tsudev-files: vòng khóa - FileeUpdatePublicKey có thể chứa nhiều khóa ngăn bởi "|" (khóa chính | khóa dự phòng
    // ngoại tuyến). Chữ ký hợp lệ với BẤT KỲ khóa nào trong vòng thì đạt. Lộ/mất khóa chính: ký bằng khóa dự phòng,
    // phát hành bản bỏ khóa cũ khỏi vòng (docs/vi/PHAT-HANH.md mục "Thay khóa ký").
    public const int MaxKeys = 4;

    public static bool Verify(byte[] data, string signature, string publicKey)
    {
        var keys = Keys(publicKey);
        if (keys.Length == 0) return false;
        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(signature.Trim());
        }
        catch (FormatException)
        {
            return false;
        }
        if (bytes.Length != 64) return false;
        foreach (var key in keys)
        {
            if (VerifyOne(data, bytes, key)) return true;
        }
        return false;
    }

    /// <summary>The keys of a key ring ("|" between them); empty when there are none or more than <see cref="MaxKeys"/>.</summary>
    internal static string[] Keys(string? publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey)) return [];
        var keys = publicKey.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return keys.Length <= MaxKeys ? keys : [];
    }

    private static bool VerifyOne(byte[] data, byte[] signature, string publicKey)
    {
        try
        {
            using var key = ECDsa.Create();
            var spki = Convert.FromBase64String(publicKey);
            key.ImportSubjectPublicKeyInfo(spki, out var read);
            if (read != spki.Length)   // không chấp nhận byte thừa sau khóa
                return false;
            if (key.ExportParameters(false).Curve.Oid.Value != ECCurve.NamedCurves.nistP256.Oid.Value)
                return false;
            return key.VerifyData(data, signature, HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
        }
        catch (Exception ex) when (ex is FormatException or CryptographicException)
        {
            return false;
        }
    }
}

internal sealed class UpdateVerificationException(string message) : IOException(message);
