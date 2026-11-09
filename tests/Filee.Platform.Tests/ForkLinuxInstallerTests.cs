// tsudev-files: script cài/gỡ Linux và macOS nói tiếng Việt mặc định và tiếng Anh khi FILEE_LANG=en (giống dòng lệnh).

using System.Diagnostics;

namespace Filee.Platform.Tests;

public sealed class ForkLinuxInstallerTests
{
    [Theory]
    [InlineData(null, "Tùy chọn không hợp lệ: --la", "Cách dùng:")]
    [InlineData("vi", "Tùy chọn không hợp lệ: --la", "Cách dùng:")]
    [InlineData("EN", "Unknown option: --la", "Usage:")]
    [InlineData("khong-ro", "Tùy chọn không hợp lệ: --la", "Cách dùng:")]
    public void Install_script_speaks_the_fork_language(string? language, string error, string usage)
    {
        Assert.SkipUnless(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS(), "Cần /bin/sh.");
        var (code, output) = Run("install.sh", language, "--la");
        Assert.Equal(1, code);
        Assert.Contains(error, output);
        Assert.Contains(usage, output);
    }

    [Fact]
    public void Every_message_of_the_scripts_has_both_languages()
    {
        foreach (var path in new[] { "linux/install.sh", "linux/uninstall.sh", "macos/install.sh", "macos/uninstall.sh" })
        {
            var lines = File.ReadAllLines(Path.Combine(InstallerFolder, "..", path));
            // Thông báo cho người dùng đi qua say "<vi>" "<en>"; printf còn lại chỉ ghi tệp đánh dấu hoặc lệnh mv để chép.
            var plain = lines.Where(l => l.TrimStart().StartsWith("printf '%s\\n' \"", StringComparison.Ordinal)
                                         && !l.Contains("upgrade_token", StringComparison.Ordinal)
                                         && !l.Contains("\"$1\"", StringComparison.Ordinal)
                                         && !l.Contains("  mv '", StringComparison.Ordinal));
            Assert.Empty(plain);
            Assert.Contains(lines, l => l.TrimStart().StartsWith("say ", StringComparison.Ordinal));
        }
    }

    [Theory]
    [InlineData("linux")]
    [InlineData("macos")]
    public void Packages_carry_the_vietnamese_install_guide(string platform)
    {
        var guide = Path.Combine(InstallerFolder, "..", platform, "HUONG-DAN-CAI-DAT.txt");
        Assert.True(File.Exists(guide), guide);
        Assert.Contains("tsudev-org/tsudev-files/releases", File.ReadAllText(guide));
        var build = File.ReadAllText(Path.Combine(InstallerFolder, "..", "..", "build", "build-portable.ps1"));
        Assert.Contains($"'installer/{platform}/HUONG-DAN-CAI-DAT.txt'", build);
    }

    private static string InstallerFolder
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Filee.slnx")))
                dir = dir.Parent;
            return Path.Combine(dir?.FullName ?? throw new DirectoryNotFoundException("Filee.slnx"), "installer", "linux");
        }
    }

    private static (int Code, string Output) Run(string script, string? language, params string[] args)
    {
        var psi = new ProcessStartInfo("/bin/sh")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        psi.ArgumentList.Add(Path.Combine(InstallerFolder, script));
        foreach (var arg in args)
            psi.ArgumentList.Add(arg);
        psi.Environment.Remove("FILEE_LANG");
        if (language is not null)
            psi.Environment["FILEE_LANG"] = language;
        using var process = Process.Start(psi)!;
        var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        process.WaitForExit(10000);
        return (process.ExitCode, output);
    }
}
