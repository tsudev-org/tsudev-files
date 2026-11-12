# PHIẾU BÀN GIAO - TsuFiles: T2 xong, T3a chờ gộp, tiếp T3b (định danh lúc chạy + di trú, bản 2.4.1)

- **Mã phiếu**: 20261113-01
- **Từ**: phiên tsudev-files-b3 (12-13/11/2026) - **Đến**: phiên kế tiếp của repo này
- **Trạng thái**: CHỜ NHẬN
- **Quyết định nguồn**: `docs/adr/0003-ten-san-pham-tsufiles.md`. Phiếu trước: `20261112-05` (mục 0 vẫn đúng).

## 0. Đọc trước

- `gh` luôn kèm `-R tsudev-org/tsudev-files`. Agent bị chặn force-push, xóa nhánh từ xa, deploy: đưa chủ dự án lệnh `!`.
- Đổi tên hàng loạt: KHÔNG ghi tệp bằng `Path.read_text()/write_text()` mặc định cho tệp CRLF (`build/*.ps1`), và
  dùng `--tep` của `tools/doi-ten-tsudev.py` cho JSON dịch (đọc/ghi lại bằng `json.dumps` làm hỏng định dạng 22 tệp).
- Thay chuỗi hiển thị bằng regex phải soát các chỗ chuỗi nằm trong mã: phiên này suýt làm gãy regex tên tệp phát hành
  `^(?:Filee|tsudev-files)-` của `UpdateService` (test `Fork_release_name_is_accepted` bắt được).
- Test App cục bộ: chạy theo `--filter`, không chạy cả bộ (bộ nhớ `local-test-budget`). Xong thì
  `dotnet format TsuFiles.slnx --no-restore --severity warn` (khoảng 15 phút, chạy nền).

## 1. Đã xong trong phiên

- **T2 - PR #49 đã gộp** (`290456b4`). CI: mọi job xanh (macOS arm64 bị hủy ở lần đầu, chạy lại thì xanh); check
  "CodeQL" đỏ chỉ vì 4 cảnh báo cũ `cs/cleartext-storage` ở `Program.cs` (dương tính giả, không có cảnh báo mới).
  - `tools/doi-ten-tsudev.py` nhận `Filee.*`/`Tsudev.Files.*`/`TsudevFiles*` và đổi thẳng sang `TsuFiles.*`.
  - Tên hiển thị TsuFiles ở app, Setup, trình cài Linux/macOS, Info.plist, GNOME/KWin, README, website, og-image.
  - Kiểu giao diện **TsuFiles Cú đêm**: `Services/NightOwlPalette.cs`, thẻ thứ 3 ở `ThemePage.axaml`, test
    `NightOwlThemeTests` (gồm ảnh chụp `bin/.../screenshots/night-owl-theme-page.png`).
- **T3a - PR #50** (bắt buộc cập nhật, phiếu hệ sinh thái `~/projects/logs/handover/20261112-04`, đã ghi kết quả vào
  phiếu đó): `Core/Updates/VersionGate.cs`, `App/Services/ForcedUpdateService.cs`, `website/public/phien-ban.json`,
  `docs/vi/CAP-NHAT-BAT-BUOC.md`. Đã gộp `main` vào nhánh sau khi #49 gộp. Xem mục 5 cho kết quả CI/gộp.

## 2. Việc tiếp theo

- [x] 2.1. Gộp #50 - xong (mục 5).
- [x] 2.2. (Xong 13/11/2026, phiên bản Worker `f30e00a3`; đã kiểm tiêu đề, logo, `phien-ban.json` 200 + max-age=300.)
      **Chủ dự án deploy website** sau khi #49/#50 gộp: `! cd website && npm run deploy`. Nghiệm thu: tiêu đề
      "TsuFiles | tsudev", logo/og-image con cú, `https://files.tsudev.com/phien-ban.json` trả
      `{"schema":1,"phienBan":"2.4.0","moiNhat":1,"toiThieu":1}`. Đổi og-image thì tải lại Social preview của repo
      (chỉ làm được qua giao diện web).
- [ ] 2.3. **T3b - định danh lúc chạy (bản 2.4.1)**, có thể chia nhiều PR: `Filee.exe` -> `TsuFiles.exe`,
      `filee-cli` -> `tsufiles-cli`, lệnh `filee` -> `tsufiles` (không bí danh), `FILEE_*` -> `TSUFILES_*`, thư mục dữ
      liệu/registry/AppGuid + di trú một lần, Setup tiếp quản `2.4.0-vn.*` và thay lối tắt/icon (`ForkDefaults.InstalledName`
      -> "tsufiles"; giữ dọn `tsudev-files.lnk`, `Filee.lnk`), ExplorerMenu/COM, GNOME (`filee-donut@filee.sh`), bundle
      macOS (`com.filee.app`, `Filee.app`), tên tệp phát hành `tsufiles-*` (`build/*.ps1`, `release.yml`,
      `packaging/package.json`, website `/tai-ve/`). Khoảng 4600 chỗ `Filee`/`filee`/`FILEE_` trong `src installer build
      packaging .github tests` (nhiều nhất: i18n, `Cli/Help.cs`, `installer/linux/install.sh`). Bước đầu nên khảo sát
      các chỗ đọc thư mục dữ liệu (`UserDataStore`, `Program.cs`, `EngineInstaller`) và viết test di trú trước.
- [ ] 2.4. Phát hành 2.4.1: `Version` 2.4.1, `ForkDefaults.BuildNumber` đã là 2, mục `## 2.4.1` trong CHANGELOG (ghi
      rõ lệnh `filee` -> `tsufiles` làm gãy script cũ), theo `docs/vi/PHAT-HANH.md`; sau khi công khai sửa
      `phien-ban.json` (`phienBan` 2.4.1, `moiNhat` 2) và nhờ deploy.
- [ ] 2.5. T4 đổi tên repo (ADR-0003 mục 3.4) - cần chủ dự án đổi trong Settings.
- [ ] 2.6. Còn từ `20261112-03`: 2.5 (cập nhật #42, #44 bằng merge `main` rồi chạy lại công cụ đổi tên - nay đổi sang
      `TsuFiles.*`), 2.8 (chủ dự án bật Code Owner review).

## 3. File đang khóa

- (không có)

## 4. Cổng kiểm

- [x] #49: build, gitleaks, website, bản dịch, quy ước, CodeQL (C#), portable 4 nền tảng, Linux native, win-arm64.
- [x] #50 cục bộ: `VersionGateTests` 33, `ForcedUpdateTests` 4, bản dịch, `check-dist` (thử cả giá trị sai).

## 5. Kết quả xử lý

- #50: 20/20 check xanh (CodeQL cũng xanh), gộp 13/11/2026, `main` @ `3f3e6fca`.
- Phiếu `20261113-02` (phiên linux-packages): thêm phụ thuộc deb/rpm (libXt, libXtst, libXrandr, libinput,
  libwayland-client) + tên `.desktop` TsuFiles - PR #53. Sau khi gộp đã nhắn phiên `linux-packages-1e`; khi kho cài
  thật xanh và xuất bản thì chuyển `apt`, `rpm` từ `planned` sang `channels` trong `packaging/package.json` + khối
  "Cài bằng lệnh" ở `/tai-ve/` (việc trong hàng đợi STATE). T3b đổi `name`/`command`/tên tệp thì báo phiên đó.
