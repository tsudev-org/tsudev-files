# PHIẾU BÀN GIAO - TsuFiles T3b-1..3 xong (dữ liệu, lệnh, tệp chạy); tiếp T3b-4 rồi phát hành 2.4.1

- **Mã phiếu**: 20261113-03
- **Từ**: phiên agent-files (13/11/2026) - **Đến**: phiên kế tiếp của repo này
- **Trạng thái**: ĐÃ NHẬN - xong T3b-4, T4 (phiếu 20261113-05)
- **Quyết định nguồn**: `docs/adr/0003-ten-san-pham-tsufiles.md` (đọc kỹ **mục 5** mới thêm). Phiếu trước: `20261113-01`.

## 0. Đọc trước

- `gh` luôn kèm `-R tsudev-org/tsufiles`. Máy KHÔNG có `jq`: dùng `gh ... --jq`.
- Agent bị chặn force-push, xóa nhánh từ xa, deploy: đưa chủ dự án lệnh `!`. Nhánh đã push: chỉ merge, không rebase.
- **Ràng buộc trình cập nhật 2.4.0-vn.1 (ADR-0003 mục 5)**: tệp phát hành PHẢI giữ tiền tố `tsudev-files-`; gói Linux
  PHẢI có tệp chạy tên `Filee` (đang là launcher gọi `TsuFiles`) cạnh `install.sh`; Windows Setup phải tiếp quản
  bản cài dưới AppGuid cũ. Phá một điều là bản 2.4.0-vn.1 đã cài im lặng không bao giờ cập nhật được.
- Test cục bộ: Core/Cli/Platform chạy được; App chạy theo `--filter`; Setup chỉ chạy trên CI Windows.

## 1. Đã xong trong phiên

- **T3b-1 (PR #57, gộp `3d598f80`)**: thư mục dữ liệu `TsuFiles` (`%APPDATA%`, `%LOCALAPPDATA%\TsuFiles\engines`, cache, nhật ký
  `tsufiles-<ngày>.log`); `Core/Settings/LegacyDataMigration.cs` chuyển một lần từ `Filee` lúc App/CLI khởi động (gộp
  an toàn, không ghi đè/xóa); biến môi trường `FILEE_*` -> `TSUFILES_*` (`tools/doi-ten-tsudev.py` có bảng ENV);
  DLL menu Explorer đọc `%APPDATA%\TsuFiles\explorer-menu.txt`; ADR-0003 mục 5; CHANGELOG mục "Chưa phát hành (2.4.1)".
- **T3b-2 (PR #58, gộp `e19d7780`)**: lệnh `tsufiles` (không bí danh), `tsufiles-cli(.exe)`, `cli\tsufiles.cmd`, `~/.local/bin/tsufiles`
  (nâng cấp gỡ liên kết `filee` chỉ khi nó trỏ vào bản cài của mình), chuỗi 21 ngôn ngữ.
- **T3b-3 (PR #59, gộp `9c11f17d`)**: assembly/tệp chạy `TsuFiles(.exe)`, `avares://TsuFiles/`; Setup `AppExeIn` nhận `Filee.exe`; cài
  mới vào thư mục `TsuFiles`; launcher `Filee` trong gói Linux; macOS `CFBundleExecutable` TsuFiles; Gửi tới `TsuFiles.lnk`.
  WM_CLASS giữ `Filee` tường minh (`Program.LinuxWindowClass`).

## 2. Việc tiếp theo

- [ ] 2.1. **T3b-4** (mỗi mục cân nhắc tương thích, có thể tách PR):
  - Linux/GNOME: UUID extension `filee-donut@filee.sh` (miền upstream) -> UUID mới, install.sh gỡ extension cũ; KWin
    script, Hyprland conf; WM_CLASS `Filee` -> `TsuFiles` (+ regex `_isDonut`, `package.json` `wmClass`).
  - Linux: thư mục cài `~/.local/share/filee/application`, marker `.filee-package`/`com.filee.app`, tệp
    `com.filee.app.desktop`, `X-Filee-Managed` - đổi phải giữ `GetLinuxInstallDirectory` cũ cho trình cập nhật 2.4.0.
  - macOS: `Filee.app` -> `TsuFiles.app`, bundle id `com.filee.app` (đổi làm mất quyền Accessibility/Automation đã cấp,
    LaunchAgent `com.filee.app.plist`, `Filee.workflow`) - cần hỏi chủ dự án có chấp nhận cấp lại quyền.
  - Windows: tên giá trị Run `Filee`, khóa `*\shell\Filee`, gói MSIX `Filee.ExplorerMenu` (publisher `CN=Filee`),
    `FileeExplorerMenu.dll`; AppGuid giữ nguyên là hợp lý (không hiển thị) - ghi quyết định vào ADR.
  - Nội bộ, có thể giữ: tên single-instance `Filee-<hash>`, marker engine `.filee-component` (đổi thì engine đã tải bị
    coi là chưa cài), tiền tố tệp tạm.
- [ ] 2.2. Lúc phát hành 2.4.1 (`docs/vi/PHAT-HANH.md`): đổi tiêu đề CHANGELOG thành `## 2.4.1`; `packaging/package.json`
  (`bin` -> `tsufiles-cli`, `gui` -> `TsuFiles.exe`, `links` -> `tsufiles`, `command`, `exec`, `formerNames`) - nhắn phiên
  `linux-packages` TRƯỚC; website (`TSUFILES_LANG`, lệnh `tsufiles`, khối "Cài bằng lệnh"): chạy
  `python3 -I tools/doi-ten-tsudev.py --kiem` (còn 3 trang website); sau khi công khai sửa `phien-ban.json`
  (`phienBan` 2.4.1, `moiNhat` 2) và nhờ deploy. Thử cập nhật thật từ bản 2.4.0-vn.1 đã cài trên Windows và Linux.
- [ ] 2.3. T4 đổi tên repo; còn từ `20261112-03`: 2.5 (#42, #44), 2.8 (Code Owner review).

## 3. File đang khóa

- (không có)

## 4. Cổng kiểm

- [x] #57, #58, #59: CI 18-20/20 xanh mỗi PR (build Windows x64/ARM64 gồm Setup 238/238, portable 4 nền tảng, Linux
  native, CodeQL, gitleaks, quy ước, bản dịch, website). Lần đầu đỏ vì test còn mong tên cũ (`filee-*.log`,
  `%TEMP%\Filee`, `make-setup.cs` đòi `Filee.exe`, test Linux thiếu `SkipUnless`) - đã sửa. `hdiutil: Resource busy`
  trên macOS là lỗi runner, chạy lại thì xanh.

## 5. Kết quả xử lý

- `main` @ `9c11f17d`. Website không đổi (mục CHANGELOG đang là "Chưa phát hành (2.4.1)", trang `/nhat-ky/` bỏ qua) -
  không cần deploy.
- Chưa kiểm được bằng tay: cập nhật thật từ 2.4.0-vn.1 đã cài (Windows Setup, Linux tar.gz) - làm trước khi phát hành
  2.4.1 (mục 2.2).
