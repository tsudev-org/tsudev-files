# PHIẾU BÀN GIAO - packaging/package.json, chứng thực build; chờ linux-packages

- **Mã phiếu**: 20261111-04
- **Từ**: phiên tsudev-files-61 (11/11/2026) - **Đến**: phiên kế tiếp của repo này
- **Trạng thái**: CHỜ NHẬN
- **Nhánh git**: `main` @ `3875c931` (gộp PR #40) + PR nhật ký này.

## 0. Đọc trước

- Phiếu `20261111-01` mục 0 (gh `-R`, ruleset, lệnh bị chặn) vẫn đúng. Riêng tài khoản gh: theo phiếu `20261111-03`
  - KHÔNG `gh auth switch`; `tsudev-tsudev` active, push được.

## 1. Đã xong

- PR #40: `packaging/package.json`; bước `actions/attest-build-provenance` v4.2.2 trong `release.yml` (job `windows`
  có quyền `id-token`, `attestations`); cách kiểm ở `docs/vi/PHAT-HANH.md` bước 5. CI 19/19 xanh.
- Gửi phiếu `~/projects/logs/handover/20261111-03_yeu-cau-linux-packages-ho-tro-tsudev-files.md` + nhắn `projects-5d`.

## 2. Việc tiếp theo

- [ ] 2.1. Khi phiên đóng gói báo xong (bộ dựng hỗ trợ `{tagVersion}`, `assets.*.sums`, `assets.*.root`,
      `linux.desktop.exec`, `linux.links`; CI cài thử Ubuntu/Debian/Fedora đạt): chuyển `apt`, `rpm` sang `channels`
      trong `packaging/package.json`; thêm khối "Cài bằng lệnh" (apt/dnf từ `pkg.tsudev.com`) vào
      `website/src/pages/tai-ve.astro`; chủ dự án deploy. Nếu CI cài thử báo thiếu thư viện: sửa `linux.deb/rpm.depends`.
- [ ] 2.2. Chủ dự án xóa nhánh đã gộp: lệnh ở phiếu `20261111-01` mục 2.1 + `feat/packaging-package-json`
      `chore/logs-ban-giao-20261111-04`; bật "Automatically delete head branches".
- [ ] 2.3. Bản 2.4.1: kiểm `gh attestation verify` lần đầu (bước attest chưa chạy thật); cân nhắc thêm tệp SHA256SUMS
      gộp mọi nền tảng (STATE hàng đợi).
- [ ] 2.4. Hàng đợi dài hạn còn nguyên (sao lưu `dl.filee.sh`, Windows ARM64 chờ upstream PR #1339 - còn OPEN,
      Authenticode).

## 3. File đang khóa

- (không có) - `logs/LOCKS.md` trống.

## 4. Cổng kiểm

- [x] `STANDARDS_VERIFY_UPSTREAM=1 ./scripts/check-standards.sh` ĐẠT. [x] CI PR #40 xanh. [x] Không monitor/tác vụ nền.

## 5. Kết quả xử lý (agent nhận điền)

-
