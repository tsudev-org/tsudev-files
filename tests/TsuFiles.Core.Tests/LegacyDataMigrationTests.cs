// TsuFiles (ADR-0003 T3b): dữ liệu của bản 2.4.0-vn.* (thư mục "Filee") sang thư mục "TsuFiles" một lần, không mất gì.

using TsuFiles.Core.Settings;

namespace TsuFiles.Core.Tests;

public class LegacyDataMigrationTests
{
    private static string Write(string path, string content = "x")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void Settings_and_engines_move_to_the_new_folders()
    {
        using var dir = new TempDir();
        var appData = Path.Combine(dir.Path, "Roaming");
        var local = Path.Combine(dir.Path, "Local");
        Write(Path.Combine(appData, "Filee", "settings.json"), "{\"theme\":1}");
        Write(Path.Combine(appData, "Filee", "logs", "filee-20261113.log"));
        Write(Path.Combine(local, "Filee", "engines", "pandoc", ".filee-component"));

        var steps = LegacyDataMigration.Run(appData, local);

        Assert.Equal(2, steps.Count);
        Assert.All(steps, s => Assert.Equal(LegacyDataMigration.Outcome.Moved, s.Outcome));
        Assert.Equal("{\"theme\":1}", File.ReadAllText(Path.Combine(appData, "TsuFiles", "settings.json")));
        Assert.True(File.Exists(Path.Combine(appData, "TsuFiles", "logs", "filee-20261113.log")));
        Assert.True(File.Exists(Path.Combine(local, "TsuFiles", "engines", "pandoc", ".filee-component")));
        Assert.False(Directory.Exists(Path.Combine(appData, "Filee")));
        // LocalAppData\Filee chỉ còn trống sau khi engines đi: xóa luôn.
        Assert.False(Directory.Exists(Path.Combine(local, "Filee")));
    }

    [Fact]
    public void Running_again_does_nothing()
    {
        using var dir = new TempDir();
        var appData = Path.Combine(dir.Path, "Roaming");
        Write(Path.Combine(appData, "Filee", "settings.json"));

        Assert.Single(LegacyDataMigration.Run(appData, ""));
        Assert.Empty(LegacyDataMigration.Run(appData, ""));
        Assert.True(File.Exists(Path.Combine(appData, "TsuFiles", "settings.json")));
    }

    [Fact]
    public void Without_old_data_nothing_is_made()
    {
        using var dir = new TempDir();
        Assert.Empty(LegacyDataMigration.Run(Path.Combine(dir.Path, "Roaming"), Path.Combine(dir.Path, "Local")));
        Assert.False(Directory.Exists(Path.Combine(dir.Path, "Roaming", "TsuFiles")));
        Assert.False(Directory.Exists(Path.Combine(dir.Path, "Local", "TsuFiles")));
    }

    [Fact]
    public void An_existing_new_folder_gets_the_missing_items_and_keeps_its_own()
    {
        using var dir = new TempDir();
        var from = Path.Combine(dir.Path, "Filee");
        var to = Path.Combine(dir.Path, "TsuFiles");
        Write(Path.Combine(from, "settings.json"), "old");
        Write(Path.Combine(from, "presets.json"), "old presets");
        Write(Path.Combine(from, "logs", "filee-20261112.log"), "old log");
        Write(Path.Combine(to, "logs", "tsufiles-20261113.log"), "new log");

        Assert.Equal(LegacyDataMigration.Outcome.Merged, LegacyDataMigration.MoveFolder(from, to));

        Assert.Equal("old", File.ReadAllText(Path.Combine(to, "settings.json")));
        Assert.Equal("old presets", File.ReadAllText(Path.Combine(to, "presets.json")));
        Assert.Equal("old log", File.ReadAllText(Path.Combine(to, "logs", "filee-20261112.log")));
        Assert.Equal("new log", File.ReadAllText(Path.Combine(to, "logs", "tsufiles-20261113.log")));
        Assert.False(Directory.Exists(from));
    }

    [Fact]
    public void A_name_in_both_places_keeps_both_files()
    {
        using var dir = new TempDir();
        var from = Path.Combine(dir.Path, "Filee");
        var to = Path.Combine(dir.Path, "TsuFiles");
        Write(Path.Combine(from, "settings.json"), "old");
        Write(Path.Combine(from, "history.json"), "old history");
        Write(Path.Combine(to, "settings.json"), "new");

        Assert.Equal(LegacyDataMigration.Outcome.Partial, LegacyDataMigration.MoveFolder(from, to));

        // Không ghi đè, không xóa: bản mới giữ chỗ mới, bản cũ ở lại chỗ cũ.
        Assert.Equal("new", File.ReadAllText(Path.Combine(to, "settings.json")));
        Assert.Equal("old", File.ReadAllText(Path.Combine(from, "settings.json")));
        Assert.Equal("old history", File.ReadAllText(Path.Combine(to, "history.json")));
        Assert.False(File.Exists(Path.Combine(from, "history.json")));
    }

    [Fact]
    public void An_override_folder_is_left_alone()
    {
        using var dir = new TempDir();
        var before = Environment.GetEnvironmentVariable(UserDataStore.DirectoryVariable);
        try
        {
            Environment.SetEnvironmentVariable(UserDataStore.DirectoryVariable, dir.Path);
            Assert.Empty(LegacyDataMigration.Run());
        }
        finally
        {
            Environment.SetEnvironmentVariable(UserDataStore.DirectoryVariable, before);
        }
    }

    [Fact]
    public void The_new_names_are_the_product_name()
    {
        Assert.Equal("TsuFiles", ForkDefaults.DataFolderName);
        Assert.Equal("Filee", ForkDefaults.LegacyDataFolderName);
        Assert.Equal("TSUFILES_DATA_DIR", UserDataStore.DirectoryVariable);
    }
}
