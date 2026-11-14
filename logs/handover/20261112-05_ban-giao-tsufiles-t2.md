# PHIẾU BÀN GIAO - Đổi tên sản phẩm thành TsuFiles: T1 xong, tiếp phase T2

- **Mã phiếu**: 20261112-05
- **Từ**: phiên tsufiles-02 (12/11/2026) - **Đến**: phiên kế tiếp của repo này
- **Trạng thái**: ĐÓNG (14/11/2026) - T2 (#49), T3 (#50, #57-#61), T4 (#62) xong; phát hành 2.4.1 ở phiếu 20261113-05
- **Quyết định nguồn**: `docs/adr/0003-ten-san-pham-tsufiles.md` (chủ dự án chốt 12/11/2026). Đọc ADR trước.

## 0. Đọc trước

- Phiếu `20261111-01` mục 0 (gh luôn `-R tsudev-org/tsufiles`, vì `gh` mặc định trỏ upstream KnifeLemon/Filee).
- Agent **bị chặn force-push** (classifier "Git Destructive") ngoài việc bị chặn deploy/xóa nhánh từ xa. Không rebase
  nhánh đã push: gộp `main` vào nhánh bằng merge commit (`git merge origin/main`) rồi push thường.
- Đổi tên/namespace xong PHẢI chạy `dotnet format tsudev-files.slnx --no-restore --severity warn` (thứ tự `using`),
  nếu không job `build` đỏ (bài học PR #46). Lệnh mất khoảng 15 phút trên WSL - chạy nền.
- Pillow cho dây chuyền logo: `python3 -m venv <scratch>/brand && <scratch>/brand/bin/pip install -r
  tools/thuong-hieu/requirements.txt` (vtracer segfault trên Python 3.14 - không dùng).

## 1. Đã xong trong phiên

- PR #46 (P5a): sửa thứ tự using (`2d02d52b`), CI xanh, đã gộp. PR #47 (P6): thêm commit logo cú (`0c92a2c9`) + merge
  P5a, đã gộp. Kết quả cụ thể: mục 5.
- **T1 logo**: ảnh gốc `assets/brand/source/tsufiles-1.webp` (ảnh bìa, cắt con cú trong `OWL_BOX`) và
  `tsufiles-2.webp` (PNG chủ dự án gửi chuyển WebP lossless, giống từng điểm ảnh). Dây chuyền
  `tools/thuong-hieu/tao-tai-san.py` tách nền chuyển sắc, sinh lại `.ico` 10 lớp, favicon, PWA, og-image. Đã xóa
  `tsufiles-1.webp`/`tsufiles-2.png` ở gốc repo (chưa từng commit).
- Con cú trong ảnh bìa chỉ cao khoảng 160 px: icon 256/512 là phóng to + làm nét. Có ảnh con cú riêng >= 1024 px hoặc
  SVG thì thay ảnh gốc (hỏi chủ dự án).
- Ảnh bìa có chữ "tsufiles.io" và "for the web" - sai (sản phẩm là app desktop, miền `files.tsudev.com`): KHÔNG đăng
  nguyên ảnh.

## 2. Việc tiếp theo (theo ADR-0003 mục 3)

- [ ] 2.1. **T2 - mã nguồn + hiển thị** (một PR từ `main`):
  - `tools/doi-ten-tsudev.py`: thêm bước `Tsudev.Files.<X>` -> `TsuFiles.<X>`, `TsudevFiles*` -> `TsuFiles*`,
    `tsudev-files.slnx` -> `TsuFiles.slnx` (vẫn nhận `Filee.*` cho bản vá upstream, đổi thẳng sang `TsuFiles.*`).
    Chạy, rồi `dotnet format`, build, `--kiem` sạch. Sửa tay AGENTS.md (B.2, B.3), CONTRIBUTING, CODEOWNERS.
  - Tên hiển thị `tsudev-files` -> `TsuFiles`: `ForkDefaults.Brand`, `Directory.Build.props` `<Product>`, i18n
    (`vi.json`, `en.json`, các ngôn ngữ khác: khóa `theme.style.Soft` đang là "Filee Soft"), Setup, README, website
    (tiêu đề, og-image chữ trong `tao-tai-san.py`, tên tệp `tsudev-files-mark.*` -> `tsufiles-mark.*`).
    **CHƯA** đổi ở T2: URL `github.com/tsudev-org/tsufiles` (T4), tên tệp phát hành `tsudev-files-<ver>-*` và
    tên exe/lệnh/thư mục dữ liệu (T3) - trình cập nhật của bản đã cài phụ thuộc.
  - Kiểu giao diện **TsuFiles Cú đêm**: thẻ thứ 3 trong `ThemePage.axaml` (cạnh Soft, Material You); bảng màu đen
    `#0A0A0A` + tím lấy từ logo (đo từ `tsufiles-2.webp`), logo cú ở thanh bên/Trang chủ/Giới thiệu; mặc định cho cài
    mới (`AppSettings`), cài cũ giữ lựa chọn. Mã: `Styles/Soft.axaml`, `Services/ThemeService.cs`,
    `Services/MaterialYouPalette.cs`, `ViewModels/Pages/ThemePageViewModel.cs`, `Core/Settings/AppSettings.cs`.
    Test + chụp màn hình.
- [ ] 2.2. **T3 - bản 2.4.1** (thay P5b): `TsuFiles.exe`, lệnh `tsufiles` (bỏ `filee`, không bí danh), `TSUFILES_*`,
      thư mục dữ liệu + di trú, Setup tiếp quản `2.4.0-vn.*` và thay lối tắt/icon cũ (ảnh chủ dự án gửi: app đã cài
      vẫn hiện icon và tên cũ), ExplorerMenu/COM, GNOME, bundle macOS, tên tệp phát hành `tsufiles-*` (trình cập nhật
      nhận cả hai tiền tố). Gộp luôn phiếu hệ sinh thái `~/projects/logs/handover/20261112-04_*` (bắt buộc cập nhật,
      `files.tsudev.com/phien-ban.json`, mẫu lotus-share PR #39) - ghi kết quả vào phiếu đó.
- [ ] 2.3. **T4 - repo**: chủ dự án đổi tên `tsudev-org/tsufiles` -> `tsudev-org/tsufiles` (Settings), rồi sửa
      URL, `git remote`, Worker `tsufiles`, thư mục cục bộ; thêm `tsudev-files` vào danh sách tên cấm tạo lại trong
      `~/.claude/CLAUDE.md` (đã có sẵn trong danh sách tsudev-tsudev - kiểm lại cho `tsudev-org`).
- [ ] 2.4. Còn từ phiếu `20261112-03`: 2.5 (rebase #42, #44 bằng merge `main`, chạy lại công cụ đổi tên), 2.8 (chủ dự
      án bật Code Owner review).

## 3. File đang khóa

- (không có)

## 4. Cổng kiểm

- [x] `tao-tai-san.py --kiem` ĐẠT. [x] `dotnet format --verify-no-changes` sạch trên P5a. [ ] CI #46/#47: mục 5.

## 5. Kết quả xử lý

- Kết quả của phiên tsufiles-02 (người nhận phiếu điền tiếp bên dưới):
  - CI #46 và #47: `build`, gitleaks, website, bản dịch, quy ước xanh. `portable (macos-15, osx-arm64)` đỏ ở #46 nhưng
    xanh ở #47 (cùng cây mã) - chập chờn. Check "CodeQL" đỏ: 4 cảnh báo cũ ở `Program.cs` (mục 1 STATE), không bắt buộc.
  - Gộp #46, #47 lúc 12/11/2026, `main` @ `39af4028`.
  - Website chưa deploy lại (agent bị chặn): chủ dự án chạy `! cd website && npm run deploy`, rồi nghiệm thu favicon
    con cú, `/ung-ho/`, og-image theo phiếu `20261112-03` mục 2.3.
