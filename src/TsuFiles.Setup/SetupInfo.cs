using System.Reflection;

namespace TsuFiles.Setup;

/// <summary>Facts about this Setup program.</summary>
public static class SetupInfo
{
    /// <summary>GitHub repository from Directory.Build.props (TsuFilesRepositoryUrl).</summary>
    public static string RepositoryUrl =>
        typeof(SetupInfo).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(a => a.Key == "RepositoryUrl")?.Value ?? "https://github.com/tsudev-org/tsufiles";

    /// <summary>The website: one big download button.</summary>
    public const string WebsiteUrl = "https://github.com/tsudev-org/tsufiles/releases/latest";

    /// <summary>The version this Setup program was built as (the payload says which Filee it installs).</summary>
    public static string Version =>
        typeof(SetupInfo).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "0.0.0";
}
