# PHIẾU BÀN GIAO - Uy tín công khai P1 + Dependabot; còn lại toàn việc của chủ dự án

- **Mã phiếu**: 20261114-03
- **Từ**: phiên agent-files (14/11/2026) - **Đến**: phiên kế tiếp của repo này
- **Trạng thái**: MỞ - mục 2 là việc 👤; agent phiên sau chỉ kiểm lại và đóng
- **Phiếu trước**: `20261113-05` (ĐÓNG), `20261113-03` (ĐÓNG phần agent). Kế hoạch tổng: `20261114-01`.

## 0. Đọc trước

- Repo `tsudev-org/tsufiles`; `gh` luôn kèm `-R tsudev-org/tsufiles`. KHÔNG tạo lại tên `tsudev-files` (ADR-0003 mục 7).
- Chủ dự án cho phép trong phiên 14/11/2026: commit/push/PR/merge/`wrangler deploy`. Phiên mới vẫn phải được nói lại
  trong chat (bộ nhớ `classifier-blocked-actions`).
- Ảnh chụp README/website sinh từ test headless (`assets/screenshots/README.md`, `website/README.md`), không chụp tay.

## 1. Đã xong trong phiên

- Gộp Dependabot #52 (NuGet), #64 (website: astro, marked, wrangler) - CI xanh toàn bộ.
- PR uy tín công khai (phiếu `20261113-03`):
  - README VI/EN: huy hiệu release/CI/MIT/Scorecard, ảnh Cú đêm (`assets/screenshots/tsufiles-*-{en,vi}.png`, test
    `NightOwlThemeTests.Main_window_renders_in_night_owl_dark_for_the_readme`), bảng Cài đặt (Scoop, Homebrew, lệnh một
    dòng `tsudev.com/install.sh`) + cách kiểm SHA256SUMS/chứng thực.
  - Ghi chú phát hành v2.4.1-vn.1 trên GitHub có "English summary" (khối `<!-- update -->` giữ nguyên); chép vào
    CHANGELOG; `docs/vi/PHAT-HANH.md` bước 1 bắt buộc mục này.
  - `.github/workflows/scorecard.yml` (OpenSSF Scorecard, SARIF category `scorecard`), `docs/best-practices.md`.
  - Issue #75, #76 (good first issue), #77, #78 (help wanted); `.github/ISSUE_TEMPLATE/config.yml`.
  - CONTRIBUTING.md: tên TsuFiles, mục Releasing trỏ về `docs/vi/PHAT-HANH.md`.
- Sửa lỗi giao diện: nhãn thanh bên dài ("Thanh công cụ vòng tròn") bị cắt mất chữ -> xuống dòng (`MainWindow.axaml`,
  MaxWidth 148 = chỗ còn lại của thanh bên 230). Đã thử nới thanh bên 260: cột phải trang Thanh công cụ bị cắt ở cỡ
  cửa sổ mặc định 1080 - đừng nới.
- Ảnh website `website/public/anh/page-*-vi.png` sinh lại (tên TsuFiles, 2.4.1, không còn nhãn bị cắt).

## 2. Việc tiếp theo (👤 = chỉ chủ dự án)

- [ ] 2.1. 👤 Đăng ký OpenSSF Best Practices: <https://www.bestpractices.dev/en/projects/new>, chép câu trả lời ở
  `docs/best-practices.md`; xong thì agent thêm huy hiệu vào README và ghi id.
- [ ] 2.2. 👤 Secret `WINGET_TOKEN` (PAT `public_repo`) -> agent nộp winget qua komac, chuyển `winget` từ `planned`
  sang `channels` trong `packaging/package.json`.
- [ ] 2.3. 👤 Cloudflare: tắt Security > Bots > JavaScript Detections cho `files.tsudev.com` (lỗi CSP console).
- [ ] 2.4. 👤 #42 (repo `tsudev-org/tsufiles-assets` cho tệp build) và #44 (khóa ký dự phòng) - xem `logs/STATE.md`.
- [ ] 2.5. 👤 SignPath (D5) - chưa đủ ngưỡng uy tín (kế hoạch `~/projects/logs/handover/20261113-02` mục 5).
- [ ] 2.6. Agent: sau lần chạy Scorecard đầu tiên, đọc điểm (`gh api` code scanning, category `scorecard`) và sửa
  hạng mục dễ (mục tiêu >= 7). Trả lời PR của người đóng góp ở #75-#78.

## 3. Cổng kiểm

`dotnet format --verify-no-changes` (tệp đổi), `check_i18n_vi.py`, `check-standards.sh` ĐẠT; test App lọc
(NightOwl, NavCollapse, RenderTests.Main_window_pages_render) đạt trên WSL; CI chạy đủ.
