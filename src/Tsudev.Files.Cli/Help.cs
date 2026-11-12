// Usage text of the "filee" command.

using System.Reflection;
using Tsudev.Files.Core.Watching;

namespace Tsudev.Files.Cli;

internal static class Help
{
    public static string Version =>
        typeof(Help).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "?";

    /// <summary>Trợ giúp theo ngôn ngữ dòng lệnh (FILEE_LANG): tiếng Việt mặc định, tiếng Anh khi FILEE_LANG=en.</summary>
    public static string Text(string? command) => Text(command, CliLanguage.Current);

    internal static string Text(string? command, string language) =>
        Tsudev.Files.Core.ForkDefaults.Brand(language == "vi" ? TextVi(command) : TextEn(command));

    private static string TextEn(string? command) => command?.ToLowerInvariant() switch
    {
        "convert" => Convert,
        "watch" => Watch,
        "formats" => "Usage: filee formats [<format>] [--json]\n\nLists every format, or what one format converts to.\n" +
                     "With --json, prints the same as JSON (for scripts and other programs).\n",
        "presets" => "Usage: filee presets [--json]\n\nLists the presets saved in Filee (built-in and your own).\n" +
                     "With --json, prints the same as JSON (for scripts and other programs).\n",
        "engines" => Engines,
        _ => General,
    };

    private static string TextVi(string? command) => command?.ToLowerInvariant() switch
    {
        "convert" => ConvertVi,
        "watch" => WatchVi,
        "formats" => "Cách dùng: filee formats [<định dạng>] [--json]\n\nLiệt kê mọi định dạng, hoặc một định dạng chuyển được sang những định dạng nào.\n" +
                     "Với --json, in cùng nội dung dưới dạng JSON (cho script và chương trình khác).\n",
        "presets" => "Cách dùng: filee presets [--json]\n\nLiệt kê các preset đã lưu trong Filee (có sẵn và do bạn tạo).\n" +
                     "Với --json, in cùng nội dung dưới dạng JSON (cho script và chương trình khác).\n",
        "engines" => EnginesVi,
        _ => GeneralVi,
    };

    private const string GeneralVi = """
        tsudev-files (bản Việt hóa của KnifeLemon/Filee): chuyển đổi tệp ngoại tuyến từ dòng lệnh.

        Cách dùng:
          filee convert <tệp hoặc thư mục...> --to <định dạng> [tùy chọn]
          filee convert <tệp hoặc thư mục...> --preset <preset> [tùy chọn]
          filee watch <thư mục> --to <định dạng> [tùy chọn]
          filee formats [<định dạng>] [--json]
          filee presets [--json]
          filee engines [--json]
          filee engines (install | remove) <engine> [--json]
          filee help [<lệnh>]
          filee version

        Ví dụ:
          filee convert anh.heic --to jpg
          filee convert *.png --to webp --quality 80 -o da-chuyen
          filee convert D:\BanScan --recursive --preset to-pdf
          filee watch D:\HopThuDen --to pdf --move-originals
          filee engines install pandoc

        Preset và engine dùng chung với ứng dụng Filee. Mã thoát: 0 không có lỗi (kể cả tệp bị bỏ qua),
        1 có tệp lỗi hoặc bị hủy, 2 sai tham số hoặc thiếu đầu vào, 3 không có gì để chuyển đổi.

        Ngôn ngữ: mặc định tiếng Việt; đặt biến môi trường FILEE_LANG=en để dùng tiếng Anh.

        """;

    private const string EnginesVi = """
        Cách dùng: filee engines [--json]
                   filee engines install <engine> [--json] [--quiet]
                   filee engines remove <engine> [--json] [--quiet]

        Liệt kê các engine tùy chọn trong Cài đặt → Engine (LibreOffice, Pandoc, FFmpeg, …), hoặc cài/gỡ một engine
        với cùng nguồn tải và cùng bước kiểm tra (SHA-256) như ứng dụng. 'remove' chỉ gỡ những gì Filee đã tải;
        bản riêng của bạn và bản tìm thấy trên máy được giữ nguyên.

        Trong JSON của danh sách, "version" là phiên bản Filee sẽ tải, kể cả với engine có trạng thái
        "owncopy" (bản riêng của bạn hoặc bản tìm thấy trên máy).

        Tùy chọn:
              --json             In kết quả dạng JSON; tiến độ ghi ra stderr, mỗi dòng một đối tượng JSON
              --quiet            Chỉ in lỗi

        """;

    private const string ConvertVi = """
        Cách dùng: filee convert <tệp hoặc thư mục...> (--to <định dạng> | --preset <preset>) [tùy chọn]

        Tệp có thể dùng ký tự đại diện (*.heic). Với thư mục, Filee chuyển đổi các tệp bên trong mà nó nhận diện được.

        Tùy chọn:
          -t, --to <định dạng>   Định dạng đích, ví dụ jpg, png, pdf, docx, mp4 (xem 'filee formats')
          -p, --preset <preset>  Một preset của ứng dụng, theo id hoặc tên (xem 'filee presets')
          -o, --output <thư mục> Lưu vào thư mục này thay vì cạnh tệp nguồn
          -r, --recursive        Bao gồm thư mục con
              --sort <thứ tự>    Thứ tự ghép tệp: name, name-desc, modified, modified-desc, created hoặc
                                 created-desc (mặc định: theo thứ tự đưa vào; thư mục/ký tự đại diện theo hệ thống tệp).
                                 Khi sắp theo tên, tệp của mỗi thư mục đứng cạnh nhau, thư mục con trước. Tệp không có
                                 ngày tạo (một số hệ thống tệp Linux) thì created dùng ngày thay đổi hoặc ngày sửa
          -q, --quality <1-100>  Chất lượng ảnh cho JPG, WebP, AVIF, …
              --pages <khoảng>   Các trang PDF, ví dụ 13-55 hoặc 1-3,8,12-15 (mặc định: theo preset)
              --overwrite        Ghi đè tệp kết quả đã có (mặc định: thêm số thứ tự)
              --skip             Bỏ qua nếu tệp kết quả đã có
              --keep-dates       Giữ ngày tạo và ngày sửa của tệp gốc cho tệp đã chuyển đổi
              --zip              Gói kết quả của mỗi tệp thành một ZIP (các tệp của Shapefile đi cùng nhau)
              --json             In kết quả dạng JSON (cho script và chương trình khác); sự kiện ghi ra stderr
              --quiet            Chỉ in lỗi

        Với --json, stdout nhận một tài liệu JSON khi xong việc. Trong lúc chạy, stderr nhận mỗi dòng một đối tượng JSON:
        {"event":"progress","source":…,"fraction":…} (0 đến 1, khoảng 150 ms một lần khi có thay đổi) và
        {"event":"file",…} khi từng tệp xong, với các trường giống mục của tệp đó trong "files".

        """;

    private static string WatchVi => FormattableString.Invariant($$"""
        Cách dùng: filee watch <thư mục> (--to <định dạng> | --preset <preset>) [tùy chọn]

        Chuyển đổi các tệp được đưa vào thư mục cho đến khi nhấn Ctrl+C. Tệp chỉ được chuyển đổi khi đã hoàn chỉnh
        (không còn tăng dung lượng hay đang mở bởi chương trình khác). Kết quả lưu vào thư mục "converted" bên trong,
        trừ khi có -o.

        Tùy chọn:
          -t, --to <định dạng>   Định dạng đích
          -p, --preset <preset>  Một preset của ứng dụng, theo id hoặc tên
          -o, --output <thư mục> Nơi lưu tệp đã chuyển đổi (mặc định: <thư mục>\converted)
          -r, --recursive        Theo dõi cả thư mục con (kết quả giữ nguyên cấu trúc thư mục con)
              --move-originals   Chuyển tệp nguồn đã xử lý vào <thư mục>\originals; tệp đang chờ sẵn
                                 được chuyển đổi ngay khi bắt đầu
              --settle <giây>    Thời gian tệp phải giữ nguyên không đổi (mặc định {{FolderWatcher.DefaultSettleTime.TotalSeconds}})
              --keep-dates       Giữ ngày tạo và ngày sửa của tệp gốc cho tệp đã chuyển đổi
              --zip              Gói kết quả của mỗi tệp thành một ZIP (các tệp của Shapefile đi cùng nhau)
              --json             In sự kiện dạng JSON, mỗi dòng một đối tượng (cho script và chương trình khác)
              --quiet            Chỉ in lỗi

        Với --json, stdout chỉ nhận sự kiện: {"event":"started",…} kèm thư mục, preset, định dạng đích và thư mục kết quả,
        {"event":"file",…} khi từng tệp xong (các trường của 'filee convert --json'), và {"event":"stopped"} khi nhấn Ctrl+C.

        """);

    private const string General = """
        tsudev-files (a fork of KnifeLemon/Filee): convert files offline from the command line.

        Usage:
          filee convert <files or folders...> --to <format> [options]
          filee convert <files or folders...> --preset <preset> [options]
          filee watch <folder> --to <format> [options]
          filee formats [<format>] [--json]
          filee presets [--json]
          filee engines [--json]
          filee engines (install | remove) <engine> [--json]
          filee help [<command>]
          filee version

        Examples:
          filee convert photo.heic --to jpg
          filee convert *.png --to webp --quality 80 -o converted
          filee convert D:\Scans --recursive --preset to-pdf
          filee watch D:\Inbox --to pdf --move-originals
          filee engines install pandoc

        Presets and engines are the ones in the Filee app. Exit codes: 0 nothing failed (skipped
        files included), 1 some failed or were cancelled, 2 wrong arguments or a missing input, 3 nothing to
        convert.

        """;

    private const string Engines = """
        Usage: filee engines [--json]
               filee engines install <engine> [--json] [--quiet]
               filee engines remove <engine> [--json] [--quiet]

        Lists the optional engines of Settings → Engines (LibreOffice, Pandoc, FFmpeg, …), or installs or removes
        one with the same download and checks as the app. 'remove' only removes what Filee downloaded; your own copy
        and one found on this computer are left alone.

        In the JSON of the list, "version" is the version Filee downloads, also for an engine whose status is
        "owncopy" (your own copy or one found on this computer).

        Options:
              --json             Print the result as JSON; progress goes to stderr, one JSON object per line
              --quiet            Print nothing but errors

        """;

    private const string Convert = """
        Usage: filee convert <files or folders...> (--to <format> | --preset <preset>) [options]

        Files can be wildcards (*.heic). Folders convert the files in them that Filee knows.

        Options:
          -t, --to <format>      Target format, e.g. jpg, png, pdf, docx, mp4 (see 'filee formats')
          -p, --preset <preset>  A preset from the app, by id or name (see 'filee presets')
          -o, --output <folder>  Save there instead of next to the sources
          -r, --recursive        Include sub folders
              --sort <order>     The order files merge in: name, name-desc, modified, modified-desc, created or
                                 created-desc (default: as given; a folder or wildcard as the file system lists it).
                                 By name, each folder's files stay together, sub folders first. Where a file has
                                 no creation date (some Linux file systems), created uses its change or modified date
          -q, --quality <1-100>  Image quality for JPG, WebP, AVIF, …
              --pages <range>    The pages of PDFs, e.g. 13-55 or 1-3,8,12-15 (default: the preset's)
              --overwrite        Replace existing output files (default: add a number)
              --skip             Leave existing output files alone
              --keep-dates       Give converted files the created and modified dates of the originals
              --zip              Save the result of each file as one ZIP (a Shapefile's files together)
              --json             Print the result as JSON (for scripts and other programs); events go to stderr
              --quiet            Print nothing but errors

        With --json, stdout gets one JSON document when the job is done. While it runs, stderr gets one JSON object per
        line: {"event":"progress","source":…,"fraction":…} (0 to 1, about every 150 ms while it changes) and
        {"event":"file",…} as each file is done, with the fields of its entry in "files".

        """;

    private static string Watch => FormattableString.Invariant($$"""
        Usage: filee watch <folder> (--to <format> | --preset <preset>) [options]

        Converts files that land in the folder until Ctrl+C. Files are converted once they are complete (no
        longer growing or open in another program). Output goes to a "converted" folder inside it unless -o is given.

        Options:
          -t, --to <format>      Target format
          -p, --preset <preset>  A preset from the app, by id or name
          -o, --output <folder>  Where converted files go (default: <folder>\converted)
          -r, --recursive        Also watch sub folders (their files keep the sub folder in the output)
              --move-originals   Move converted sources to <folder>\originals; files already waiting are
                                 converted at start
              --settle <seconds> How long a file must stay unchanged (default {{FolderWatcher.DefaultSettleTime.TotalSeconds}})
              --keep-dates       Give converted files the created and modified dates of the originals
              --zip              Save the result of each file as one ZIP (a Shapefile's files together)
              --json             Print events as JSON instead, one object per line (for scripts and other programs)
              --quiet            Print nothing but errors

        With --json, stdout gets nothing but events: {"event":"started",…} with the folder, preset, target and output
        folder, {"event":"file",…} as each file is done (the fields of 'filee convert --json'), and
        {"event":"stopped"} on Ctrl+C.

        """);
}
