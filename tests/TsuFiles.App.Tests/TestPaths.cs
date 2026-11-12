// Where the repository is, for tests that read files kept in it (README, website data, generated brand assets).

namespace TsuFiles.App.Tests;

internal static class TestPaths
{
    public static readonly string RepoRoot = Find();

    private static string Find()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "TsuFiles.slnx")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new DirectoryNotFoundException("TsuFiles.slnx");
    }
}
