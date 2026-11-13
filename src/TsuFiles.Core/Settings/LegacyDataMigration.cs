// TsuFiles (ADR-0003 T3b, bản 2.4.1): chuyển dữ liệu của bản đã cài từ thư mục mang tên cũ "Filee" sang "TsuFiles"
// một lần, lúc ứng dụng hoặc dòng lệnh khởi động (trước khi có gì ghi vào thư mục mới):
//   %APPDATA%\Filee            -> %APPDATA%\TsuFiles            (cài đặt, preset, lịch sử, nhật ký)
//   %LOCALAPPDATA%\Filee\engines -> %LOCALAPPDATA%\TsuFiles\engines (engine đã tải, có thể vài GB)
// (macOS/Linux: cùng hai thư mục theo Environment.SpecialFolder.) Tệp làm việc tạm (WorkFolders) không chuyển.
//
// Cách chuyển an toàn khi chạy lặp lại hay chạy song song (ứng dụng và dòng lệnh cùng lúc):
// - thư mục mới chưa có: đổi tên cả thư mục (một thao tác, cùng ổ đĩa);
// - thư mục mới đã có (lần trước dở dang, hoặc bị khóa tệp): gộp từng mục còn thiếu; mục trùng tên thì chỗ mới giữ
//   nguyên và bản cũ ở lại chỗ cũ (không xóa dữ liệu nào);
// - chỗ cũ trống thì xóa. Không bao giờ ném ngoại lệ: dữ liệu cũ còn nguyên ở chỗ cũ nếu không chuyển được.
// TSUFILES_DATA_DIR (bản sao phát triển/kiểm thử) thì không chuyển gì.

namespace TsuFiles.Core.Settings;

public static class LegacyDataMigration
{
    /// <summary>Kết quả cho một thư mục.</summary>
    public enum Outcome
    {
        /// <summary>Không có dữ liệu cũ.</summary>
        NothingToDo,

        /// <summary>Đã đổi tên cả thư mục.</summary>
        Moved,

        /// <summary>Đã gộp vào thư mục mới có sẵn; mọi mục đã sang chỗ mới.</summary>
        Merged,

        /// <summary>Còn mục không chuyển được (đang bị khóa, không có quyền): để lại ở chỗ cũ, lần chạy sau thử lại.</summary>
        Partial,
    }

    /// <summary>Thư mục cũ và mới của một lần chuyển.</summary>
    public sealed record Step(string From, string To, Outcome Outcome);

    /// <summary>
    /// Chuyển dữ liệu người dùng và engine đã tải sang thư mục TsuFiles. Gọi đầu tiên trong Main của ứng dụng và dòng
    /// lệnh. Trả về các bước đã làm (để ghi nhật ký khi đã có logger).
    /// </summary>
    public static IReadOnlyList<Step> Run()
    {
        if (UserDataStore.OverrideDirectory is not null)
            return [];
        return Run(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                   Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
    }

    /// <summary>Như <see cref="Run()"/> với thư mục gốc chỉ định (kiểm thử).</summary>
    public static IReadOnlyList<Step> Run(string appData, string localAppData)
    {
        var steps = new List<Step>();
        if (!string.IsNullOrEmpty(appData))
        {
            var from = Path.Combine(appData, ForkDefaults.LegacyDataFolderName);
            var to = Path.Combine(appData, ForkDefaults.DataFolderName);
            steps.Add(new Step(from, to, MoveFolder(from, to)));
        }
        if (!string.IsNullOrEmpty(localAppData))
        {
            var oldRoot = Path.Combine(localAppData, ForkDefaults.LegacyDataFolderName);
            var from = Path.Combine(oldRoot, "engines");
            var to = Path.Combine(localAppData, ForkDefaults.DataFolderName, "engines");
            steps.Add(new Step(from, to, MoveFolder(from, to)));
            DeleteIfEmpty(oldRoot);
        }
        return steps.Where(s => s.Outcome != Outcome.NothingToDo).ToList();
    }

    /// <summary>Chuyển <paramref name="from"/> sang <paramref name="to"/> (xem chú thích đầu tệp).</summary>
    internal static Outcome MoveFolder(string from, string to)
    {
        try
        {
            if (!Directory.Exists(from) || SameFolder(from, to))
                return Outcome.NothingToDo;
            if (!Directory.Exists(to) && !File.Exists(to))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(to)!);
                    Directory.Move(from, to);
                    return Outcome.Moved;
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    // Một tệp bên trong đang mở (Windows), khác ổ đĩa, hoặc tiến trình kia vừa tạo thư mục mới: gộp.
                    if (!Directory.Exists(from))
                        return Directory.Exists(to) ? Outcome.Moved : Outcome.NothingToDo;
                }
            }
            var complete = Merge(from, to);
            DeleteIfEmpty(from);
            return complete && !Directory.Exists(from) ? Outcome.Merged : Outcome.Partial;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException
                                       or NotSupportedException)
        {
            return Outcome.Partial;
        }
    }

    /// <summary>Đưa từng mục của <paramref name="from"/> còn thiếu ở <paramref name="to"/> sang; true khi không sót mục nào.</summary>
    private static bool Merge(string from, string to)
    {
        Directory.CreateDirectory(to);
        var complete = true;
        foreach (var entry in new DirectoryInfo(from).EnumerateFileSystemInfos())
        {
            var target = Path.Combine(to, entry.Name);
            try
            {
                // Liên kết tượng trưng: chuyển chính liên kết, không đi theo nó.
                if (entry is DirectoryInfo dir && entry.LinkTarget is null)
                {
                    if (Directory.Exists(target))
                    {
                        complete &= Merge(dir.FullName, target);
                        DeleteIfEmpty(dir.FullName);
                    }
                    else if (!File.Exists(target))
                    {
                        dir.MoveTo(target);
                    }
                    // Chỗ mới có tệp trùng tên thư mục: giữ chỗ mới, để lại chỗ cũ.
                    else complete = false;
                }
                else if (!File.Exists(target) && !Directory.Exists(target))
                {
                    if (entry is FileInfo file) file.MoveTo(target);
                    else ((DirectoryInfo)entry).MoveTo(target);
                }
                // Trùng tên với mục đã có ở chỗ mới: giữ cả hai (không xóa dữ liệu nào), bản cũ ở lại chỗ cũ.
                else complete = false;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                complete = false;
            }
        }
        return complete;
    }

    private static void DeleteIfEmpty(string folder)
    {
        try
        {
            if (Directory.Exists(folder) && !Directory.EnumerateFileSystemEntries(folder).Any())
                Directory.Delete(folder);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Còn bị giữ: lần sau.
        }
    }

    private static bool SameFolder(string a, string b) =>
        string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(a)),
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(b)),
            OperatingSystem.IsLinux() ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
}
