// Where "Sponsor" leads: the sidebar button and the About page open a window with these. The ways to support
// TsuFiles are listed in one place only, the support page of files.tsudev.com (website/src/data/ung-ho.json), so
// new ways are added there without a new release of the app.

using TsuFiles.Core;

namespace TsuFiles.App.Services;

/// <summary>One way to support TsuFiles.</summary>
/// <param name="Id">Also the suffix of its description string, <c>sponsor.note.{Id}</c>.</param>
/// <param name="Icon">Key of the icon geometry (Styles/Icons.axaml).</param>
/// <param name="Color">Colour behind the white icon.</param>
public sealed record SponsorLink(string Id, string Name, string Url, string Icon, string Color);

public static class SponsorLinks
{
    /// <summary>The support page: every way to support TsuFiles, kept up to date without a new release.</summary>
    public const string SupportPageUrl = ForkDefaults.SupportPageUrl;

    private static readonly SponsorLink Page = new("page", "files.tsudev.com/ung-ho", SupportPageUrl, "Icon.Heart", "#7C3AED");
    private static readonly SponsorLink Star = new("star", "GitHub", ForkDefaults.RepositoryUrl, "Icon.GitHub", "#24292F");

    /// <summary>Every way, the same in every language (the support page itself is where the choice is made).</summary>
    public static IReadOnlyList<SponsorLink> For(string language) => [Page, Star];
}
