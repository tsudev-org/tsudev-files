// Entry point of tsufiles-cli.exe (run as "tsufiles" through cli\tsufiles.cmd). Ctrl+C cancels the running conversion or
// stops watching; a second Ctrl+C ends the process at once.

using System.Text;
using TsuFiles.Cli;
using TsuFiles.Core.Settings;

Console.OutputEncoding = Encoding.UTF8;
// TsuFiles (ADR-0003 T3b): dữ liệu của bản 2.4.0-vn.* nằm ở thư mục "Filee"; chuyển một lần (không ghi gì ra màn hình,
// đầu ra của lệnh có thể đang được script đọc).
LegacyDataMigration.Run();
using var cancel = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    if (cancel.IsCancellationRequested)
        return;
    e.Cancel = true;
    cancel.Cancel();
};
return await Cli.RunAsync(args, Console.Out, Console.Error, cancel.Token);
