// "Sponsor" opens a window that only leads to tsudev: the support page of files.tsudev.com (the one place that lists
// the ways to support tsudev-files) and the repository on GitHub. Nothing may lead to the original project's channels.

using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Tsudev.Files.App.Services;
using Tsudev.Files.App.Views;
using Tsudev.Files.Core;
using Tsudev.Files.Core.Localization;

namespace Tsudev.Files.App.Tests;

public class SponsorTests
{
    private static readonly string[] Allowed = ["https://files.tsudev.com/", "https://github.com/tsudev-org/"];

    [Theory]
    [InlineData("vi")]
    [InlineData("en")]
    [InlineData("ko")]
    public void Every_way_leads_to_tsudev_and_the_support_page_comes_first(string language)
    {
        var ways = SponsorLinks.For(language);

        Assert.Equal(ForkDefaults.SupportPageUrl, ways[0].Url);
        Assert.Equal("https://files.tsudev.com/ung-ho/", ways[0].Url);
        Assert.All(ways, w => Assert.Contains(Allowed, prefix => w.Url.StartsWith(prefix, StringComparison.Ordinal)));
        Assert.Equal(ways.Count, ways.Select(w => w.Url).Distinct().Count());
    }

    [Fact]
    public void No_funding_link_points_to_the_original_project()
    {
        var root = TestPaths.RepoRoot;
        var files = new[] { ".github/FUNDING.yml", "README.md", "README.en.md", "website/src/data/ung-ho.json" };
        foreach (var file in files)
        {
            var text = File.ReadAllText(Path.Combine(root, file));
            foreach (var old in new[] { "sponsors/KnifeLemon", "patreon.com/KnifeLemon", "ko-fi.com/knifelemon", "paypal.me/knifelemon", "fairy.hada.io" })
                Assert.DoesNotContain(old, text, StringComparison.OrdinalIgnoreCase);
        }
        Assert.Contains(ForkDefaults.SupportPageUrl, File.ReadAllText(Path.Combine(root, ".github/FUNDING.yml")));
    }

    [AvaloniaTheory]
    [InlineData("vi")]
    [InlineData("en")]
    [InlineData("zh-CN")]
    public void Every_way_has_its_icon_and_note(string language)
    {
        TestServices.EnsureInitialized(language);
        var window = SponsorWindow.Create(AppHost.Get<ILocalizer>());

        var ways = Assert.IsAssignableFrom<IEnumerable<SponsorWay>>(window.Ways.ItemsSource).ToList();
        Assert.Equal(2, ways.Count);
        Assert.All(ways, w =>
        {
            Assert.IsAssignableFrom<Geometry>(w.Icon);
            Assert.False(string.IsNullOrWhiteSpace(w.Note));
            Assert.DoesNotContain("sponsor.note.", w.Note);
        });
    }
}
