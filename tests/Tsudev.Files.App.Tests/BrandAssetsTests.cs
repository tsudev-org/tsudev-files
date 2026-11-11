// The icons are generated from one source image by tools/thuong-hieu/tao-tai-san.py (BRAND_ASSETS.md sections 6, 7, 12):
// every one must be there, at its size, and the .ico files must carry every layer Windows and browsers pick from.

using System.Buffers.Binary;

namespace Tsudev.Files.App.Tests;

public class BrandAssetsTests
{
    private static readonly int[] IcoLayers = [16, 20, 24, 32, 40, 48, 64, 96, 128, 256];

    [Theory]
    [InlineData("src/Tsudev.Files.App/Assets/Icons/filee.ico")]
    [InlineData("website/public/favicon.ico")]
    public void Icons_carry_every_layer(string path)
    {
        var data = File.ReadAllBytes(Path.Combine(TestPaths.RepoRoot, path));
        Assert.Equal(1, BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(2)));
        var count = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(4));
        var sizes = Enumerable.Range(0, count).Select(i => data[6 + 16 * i] == 0 ? 256 : data[6 + 16 * i]).Order();
        Assert.Equal(IcoLayers, sizes);
    }

    [Theory]
    [InlineData("src/Tsudev.Files.App/Assets/Icons/filee.png", 512, 512)]
    [InlineData("assets/brand/tsudev-files-mark-512.png", 512, 512)]
    [InlineData("website/public/favicon-16x16.png", 16, 16)]
    [InlineData("website/public/favicon-32x32.png", 32, 32)]
    [InlineData("website/public/favicon-96x96.png", 96, 96)]
    [InlineData("website/public/apple-touch-icon.png", 180, 180)]
    [InlineData("website/public/android-chrome-192x192.png", 192, 192)]
    [InlineData("website/public/android-chrome-512x512.png", 512, 512)]
    [InlineData("website/public/android-chrome-maskable-192x192.png", 192, 192)]
    [InlineData("website/public/android-chrome-maskable-512x512.png", 512, 512)]
    [InlineData("website/public/og-image.png", 1200, 630)]
    public void Pngs_have_their_size(string path, int width, int height)
    {
        var data = File.ReadAllBytes(Path.Combine(TestPaths.RepoRoot, path));
        Assert.Equal(width, BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(16)));
        Assert.Equal(height, BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(20)));
    }

    [Fact]
    public void The_app_icon_has_a_transparent_background()
    {
        using var bitmap = SkiaSharp.SKBitmap.Decode(Path.Combine(TestPaths.RepoRoot, "src/Tsudev.Files.App/Assets/Icons/filee.png"));
        Assert.Equal(0, bitmap.GetPixel(0, 0).Alpha);
        Assert.Equal(0, bitmap.GetPixel(bitmap.Width - 1, bitmap.Height - 1).Alpha);
        Assert.Equal(255, bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2).Alpha);
    }
}
