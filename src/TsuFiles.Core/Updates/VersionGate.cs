// TsuFiles: bắt buộc cập nhật theo chuẩn chung của hệ sinh thái tsudev (phiếu ~/projects/logs/handover/20261112-04, mẫu
// tsudev-org/lotus-share#39). Tệp https://files.tsudev.com/phien-ban.json = {"schema":1,"phienBan":"x.y.z","moiNhat":n,
// "toiThieu":m}; so bằng SỐ BUILD nguyên tăng dần (ForkDefaults.BuildNumber), không so chuỗi phiên bản. Quy trình phát
// hành: docs/vi/CAP-NHAT-BAT-BUOC.md.
//
// An toàn: URL cố định trong mã (không lấy URL từ nội dung tệp), không theo chuyển hướng, tối đa 16 KB, quá 8 giây thì bỏ.
// Tệp sai khuôn hay không tải được: coi như không có (ứng dụng chạy bình thường), nhưng "toiThieu" lớn nhất từng thấy
// được lưu lại (chỉ tăng) nên bản quá cũ vẫn bị chặn khi mất mạng.

using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TsuFiles.Core.Updates;

/// <summary>Nội dung hợp lệ của phien-ban.json.</summary>
/// <param name="Version">Phiên bản mới nhất, dạng x.y.z (chỉ để hiển thị).</param>
/// <param name="Latest">Số build của bản mới nhất.</param>
/// <param name="Minimum">Số build thấp nhất còn được dùng; bản thấp hơn bị chặn.</param>
public sealed record VersionFile(string Version, int Latest, int Minimum);

/// <summary>Kết luận cho bản đang chạy.</summary>
public enum VersionGateResult
{
    /// <summary>Chưa biết (không tải được tệp và chưa từng thấy mức tối thiểu cao hơn bản này).</summary>
    Unknown,

    /// <summary>Đang là bản mới nhất (hoặc mới hơn).</summary>
    UpToDate,

    /// <summary>Có bản mới, bản này vẫn dùng được.</summary>
    Optional,

    /// <summary>Bản này thấp hơn mức tối thiểu: chặn tới khi cập nhật.</summary>
    Required,
}

public static partial class VersionGate
{
    /// <summary>Địa chỉ cố định của tệp phiên bản.</summary>
    public static readonly Uri Url = new(ForkDefaults.ProductPageUrl + "/phien-ban.json");

    /// <summary>Trang tải bản mới (cố định, không lấy từ tệp phiên bản).</summary>
    public const string DownloadPageUrl = ForkDefaults.ProductPageUrl + "/tai-ve/";

    public const int MaxBytes = 16 * 1024;

    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(8);

    [GeneratedRegex(@"^\d{1,4}\.\d{1,4}\.\d{1,4}$", RegexOptions.CultureInvariant)]
    private static partial Regex VersionPattern();

    /// <summary>Đọc tệp phiên bản; null khi sai khuôn (JSON hỏng, thiếu trường, schema khác 1, số build vô lý).</summary>
    public static VersionFile? Parse(ReadOnlySpan<byte> json)
    {
        if (json.Length is 0 or > MaxBytes)
            return null;
        try
        {
            using var document = JsonDocument.Parse(json.ToArray());
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object
                || !root.TryGetProperty("schema", out var schema) || schema.ValueKind != JsonValueKind.Number
                || !schema.TryGetInt32(out var schemaVersion) || schemaVersion != 1
                || !root.TryGetProperty("phienBan", out var version) || version.ValueKind != JsonValueKind.String
                || !root.TryGetProperty("moiNhat", out var latest) || latest.ValueKind != JsonValueKind.Number
                || !root.TryGetProperty("toiThieu", out var minimum) || minimum.ValueKind != JsonValueKind.Number
                || !latest.TryGetInt32(out var latestBuild) || !minimum.TryGetInt32(out var minimumBuild))
                return null;
            var text = version.GetString()!;
            if (!VersionPattern().IsMatch(text) || minimumBuild < 1 || latestBuild < minimumBuild)
                return null;
            return new VersionFile(text, latestBuild, minimumBuild);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Kết luận cho bản có số build <paramref name="currentBuild"/>, từ tệp vừa tải (<paramref name="file"/>, có thể
    /// null) và mức tối thiểu đã lưu (<paramref name="storedMinimum"/>, 0 nếu chưa có).
    /// </summary>
    public static VersionGateResult Decide(int currentBuild, VersionFile? file, int storedMinimum)
    {
        var minimum = Math.Max(storedMinimum, file?.Minimum ?? 0);
        if (minimum > currentBuild)
            return VersionGateResult.Required;
        if (file is null)
            return VersionGateResult.Unknown;
        return file.Latest > currentBuild ? VersionGateResult.Optional : VersionGateResult.UpToDate;
    }

    /// <summary>Mức tối thiểu mới để lưu: chỉ tăng, không bao giờ giảm (tệp bị sửa lùi không mở khóa bản cũ).</summary>
    public static int RaiseMinimum(int storedMinimum, VersionFile? file) => Math.Max(storedMinimum, file?.Minimum ?? 0);

    /// <summary>
    /// Tải và đọc tệp phiên bản. Mọi lỗi (mạng, quá giờ, mã khác 200, chuyển hướng, quá cỡ, sai khuôn) trả về null.
    /// <paramref name="handler"/> chỉ dành cho test; mặc định là HttpClientHandler không theo chuyển hướng.
    /// </summary>
    public static async Task<VersionFile?> FetchAsync(HttpMessageHandler? handler = null, Uri? url = null,
        CancellationToken cancellationToken = default)
    {
        handler ??= new HttpClientHandler { AllowAutoRedirect = false };
        using var http = new HttpClient(handler, disposeHandler: true) { Timeout = Timeout, MaxResponseContentBufferSize = MaxBytes };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("TsuFiles/" + ForkDefaults.BuildNumber.ToString(CultureInfo.InvariantCulture));
        try
        {
            using var response = await http.GetAsync(url ?? Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;
            if (response.Content.Headers.ContentLength is > MaxBytes)
                return null;
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var buffer = new byte[MaxBytes + 1];
            var read = 0;
            while (read < buffer.Length)
            {
                var n = await stream.ReadAsync(buffer.AsMemory(read), cancellationToken).ConfigureAwait(false);
                if (n == 0)
                    break;
                read += n;
            }
            return read > MaxBytes ? null : Parse(buffer.AsSpan(0, read));
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException or IOException or InvalidOperationException)
        {
            return null;
        }
    }
}
