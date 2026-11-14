# PHIẾU BÀN GIAO - TsuFiles T3b-4 + T4 xong (định danh, repo tsufiles); tiếp: việc của chủ dự án rồi phát hành 2.4.1

- **Mã phiếu**: 20261113-05
- **Từ**: phiên agent-files (13/11/2026) - **Đến**: phiên kế tiếp của repo này
- **Trạng thái**: ĐANG LÀM (14/11/2026) - 2.1 deploy, 2.3 phát hành xong; còn việc chủ dự án ở 2.1, 2.2, 2.4
- **Quyết định nguồn**: `docs/adr/0003-ten-san-pham-tsufiles.md` mục 5 (ràng buộc trình cập nhật 2.4.0-vn.1),
  mục 6 (T3b-4: đổi gì, GIỮ gì), mục 7 (T4: đổi tên repo). Phiếu trước: `20261113-03_ban-giao-tsufiles-t3b.md`.

## 0. Đọc trước

- Repo giờ là **`tsudev-org/tsufiles`**: `gh` luôn kèm `-R tsudev-org/tsufiles`. KHÔNG BAO GIỜ tạo lại repo/fork tên
  `tsudev-files` trong `tsudev-org` (chuyển hướng mất, bản 2.4.0-vn.1 đã cài hỏng cập nhật) - đã ghi `~/.claude/CLAUDE.md`.
- Máy KHÔNG có `jq`: dùng `gh ... --jq`. Agent bị chặn force-push, xóa nhánh từ xa, deploy: đưa chủ dự án lệnh `!`.
- Tệp phát hành GIỮ tiền tố `tsudev-files-` (`ForkDefaults.ReleaseFilePrefix` là hằng chữ, có chú thích GIỮ NGUYÊN);
  gói Linux GIỮ launcher `Filee`; thư mục cài Linux GIỮ `~/.local/share/filee/application` + `.filee-package`.

## 1. Đã xong trong phiên

- **T3b-4 (PR #61)**: macOS `TsuFiles.app` / `com.tsudev.tsufiles` (install.sh gỡ `Filee.app` cũ có id `com.filee.app`
  + LaunchAgent cũ; người dùng phải cấp lại quyền Accessibility/Automation - chủ dự án đã chấp nhận); Linux extension
  `tsufiles-donut@tsudev.com` (install.sh gỡ `filee-donut@filee.sh` nếu là của mình và giữ trạng thái bật), KWin
  `tsufiles-donut`, Hyprland `tsufiles-donut.conf` (+ bản cũ deprecated), WM_CLASS `TsuFiles`,
  `com.tsudev.tsufiles.desktop` (`StartupWMClass=TsuFiles`), autostart/Thunar/Nemo/kio/Nautilus `tsufiles`; Windows giá
  trị Run và khóa `*\shell\TsuFiles`, lối tắt `TsuFiles.lnk` (dọn `Filee.lnk`, `tsudev-files.lnk`).
- **T4 (PR #62)**: `gh repo rename` -> `tsudev-org/tsufiles` (secret scanning, push protection, ruleset 26937532 giữ
  nguyên; URL cũ kể cả API `releases/latest` chuyển hướng 301 - đã kiểm). URL trong mã, i18n, tài liệu, website,
  `packaging/package.json` (`release.repo`) đã đổi; `logs/`, CHANGELOG bản cũ giữ nguyên.
- **PR #63**: Worker Cloudflare đổi tên `tsufiles` (`website/wrangler.jsonc`), CHƯA deploy (xem 2.1).
- **PR #65** (phiếu `20261113-07` rà quét giao diện, kết quả ở cuối bản sao phiếu): id tiêu đề duy nhất ở `/nhat-ky/`,
  menu di động một hàng. Lỗi CSP trên console mọi trang là script Cloudflare JavaScript Detections chèn vào (xem 2.1).
- Kênh cài đặt (phiếu hệ sinh thái `~/projects/logs/handover/20261113-04_*`): phiên `linux-packages-19` đã đổi URL repo
  ở linux-packages#25, scoop-bucket#8, homebrew-tap#5. Phần đổi tên gói/cask/manifest CHỜ tag 2.4.1.

## 2. Việc tiếp theo

- [x] 2.1. (Worker cũ `tsudev-files` ĐÃ XÓA 14/11/2026, trang vẫn 200) **Chủ dự án** deploy Worker mới (XONG 14/11/2026, version `5fa9213e`, `files.tsudev.com` đã trỏ Worker `tsufiles`) rồi xóa Worker cũ:
  `! cd website && npm run deploy` (trả lời Yes khi wrangler hỏi chuyển miền `files.tsudev.com`), mở
  https://files.tsudev.com kiểm tra, rồi `! cd website && npx wrangler delete --name tsudev-files`.
  Cùng lúc: tắt **Security > Bots > JavaScript Detections** của zone tsudev.com (hoặc ít nhất cho `files.tsudev.com`) -
  script `cdn-cgi/challenge-platform/.../jsd/main.js` bị CSP `script-src 'self'` chặn, báo lỗi console trên mọi trang.
  Deploy xong chạy lại `node ~/projects/logs/tools/quet-giao-dien.cjs https://files.tsudev.com / ... --out <thư mục>`.
- [ ] 2.2. **Chủ dự án** đổi tên thư mục cục bộ (đóng mọi phiên Claude đang mở ở thư mục này trước):
  `mv ~/projects/tsudev-files ~/projects/tsufiles` rồi chép bộ nhớ phiên:
  `cp -r ~/.claude/projects/-home-devtsuitstmy-projects-tsudev-files/memory ~/.claude/projects/-home-devtsuitstmy-projects-tsufiles/`.
  Remote git đã là `https://github.com/tsudev-org/tsufiles.git`. Cập nhật đường dẫn trong phiếu hệ sinh thái nếu có.
- [x] 2.3. (XONG 14/11/2026: #70 phiên bản, thẻ `v2.4.1-vn.1` công khai, #71 packaging/website/phien-ban.json; ký
  SHA256SUMS, `gh attestation verify` + SBOM đạt; nâng cấp thật Linux đạt; Windows/macOS chỉ có test CI - chủ dự án
  duyệt công khai) Phát hành 2.4.1 (`docs/vi/PHAT-HANH.md`): tiêu đề CHANGELOG `## Chưa phát hành (2.4.1)` -> `## 2.4.1`;
  `packaging/package.json`: `bin` `tsufiles-cli(.exe)`, `gui` `TsuFiles.exe`, `app` `TsuFiles.app`
  (`TsuFiles.app/Contents/MacOS/tsufiles-cli`), `links` `{"tsufiles": "tsufiles-cli"}`, `command`, `exec`
  `TsuFiles --convert %F`, `wmClass` `TsuFiles`, `linux.formerNames: ["tsudev-files"]` - CHỈ đổi cùng tag 2.4.1; website
  3 trang `dong-lenh`, `huong-dan`, `tai-ve` (`TSUFILES_LANG`, lệnh `tsufiles`) - `python3 -I tools/doi-ten-tsudev.py --kiem`
  phải sạch; sau khi công khai sửa `phien-ban.json` (`phienBan` 2.4.1, `moiNhat` 2) và nhờ deploy. Thử cập nhật thật từ
  bản 2.4.0-vn.1 đã cài trên Windows, Linux, macOS (kiểm `Filee.app` được thay bằng `TsuFiles.app`, extension GNOME
  chuyển sang id mới). Xong tag thì nhắn phiên `linux-packages` (phiếu `20261113-04` mục 3).
- [ ] 2.4. Dependabot #64 (astro 7.4.8, marked, wrangler - CI xanh) và #52 (NuGet): quy tắc nhánh chặn agent gộp, cần
  chủ dự án duyệt/gộp.
- [ ] 2.5. Còn từ `20261112-03`: 2.5 (#42 chờ repo assets, #44 chờ khóa dự phòng), 2.8 (Code Owner review).
