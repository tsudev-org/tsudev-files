# PHIẾU YÊU CẦU - Tăng uy tín công khai (tsudev-files)

- **Mã phiếu**: 20261113-03
- **Từ**: phiên `tsudev-06` (repo `tsudev`), chủ dự án yêu cầu 13/11/2026 - **Đến**: phiên kế tiếp của repo `tsudev-files`
- **Trạng thái**: ĐÓNG (14/11/2026) - phần agent của P1 xong (xem Kết quả); việc 👤 (Best Practices, winget, P2) theo dõi ở kế hoạch `~/projects/logs/handover/20261113-02` và `logs/STATE.md`

Kế hoạch đầy đủ (nguyên tắc, giai đoạn, ngưỡng, công cụ đo): `~/projects/logs/handover/20261113-02_ke-hoach-uy-tin-he-sinh-thai.md`.
ĐỌC mục 0 (nguyên tắc - chỉ tín hiệu thật, miễn phí trước) trước khi làm.

## Phần của repo này (mục 4 của kế hoạch)

Chủ lực ưu tiên 1: README EN đã có - thêm ảnh/GIF + khối Install bằng lệnh một dòng; P1.2 ghi chú phát hành tiếng Anh + SHA256SUMS; P1.3 Scorecard; P1.5 good first issue; Discussions đã bật; bổ sung winget.

Đo trước/sau: `cd ~/projects/tsudev && npm run he-sinh-thai:tin-hieu`. Xong mục nào thì đánh dấu ở mục 4 + ghi một dòng
vào mục 7 của kế hoạch, và ghi kết quả dưới đây.

## Kết quả xử lý (phiên nhận điền)

- 14/11/2026, phiên agent-files (PR `feat/uy-tin-cong-khai`):
  - P1.1 XONG: README VI + EN có huy hiệu (release, CI, MIT, Scorecard), ảnh chụp Cú đêm sinh từ test headless
    (`assets/screenshots/`, cách sinh lại ở `assets/screenshots/README.md`), khối Cài đặt/Install (Scoop, Homebrew,
    lệnh một dòng `tsudev.com/install.sh`) + cách kiểm `SHA256SUMS`/`gh attestation verify`. GIỮ `README.md` tiếng Việt
    (quy ước ECOSYSTEM_IDENTITY yêu cầu README.md có hạng sản phẩm; website ánh xạ README.md); EN ở `README.en.md`.
  - P1.2 XONG: ghi chú phát hành v2.4.1-vn.1 có mục "English summary" (sửa trên GitHub, khối `<!-- update -->` giữ
    nguyên), chép vào CHANGELOG; `docs/vi/PHAT-HANH.md` bước 1 bắt buộc mục này cho mọi bản sau. SHA256SUMS ký +
    chứng thực build + SBOM đã có từ 2.4.1.
  - P1.3 XONG phần agent: `.github/workflows/scorecard.yml` (action ghim SHA, đã đối chiếu tag v2.5.0). Điểm: xem
    <https://scorecard.dev/viewer/?uri=github.com/tsudev-org/tsufiles> sau lần chạy đầu trên `main`.
  - P1.4 chuẩn bị: `docs/best-practices.md` (câu trả lời mức passing kèm bằng chứng) - 👤 đăng ký bestpractices.dev.
  - P1.5 XONG: issue #75 (dịch chuỗi mới, good first issue), #76 (tài liệu tên TsuFiles, good first issue), #77 (bản
    EN kiến trúc bảo mật, help wanted), #78 (gói Windows ARM64, help wanted). Thêm `.github/ISSUE_TEMPLATE/config.yml`.
  - Số đo 14/11/2026 (gh api): 0 sao, 0 fork, sức khỏe cộng đồng 87% (thiếu `issue_template` theo GitHub), 575 lượt
    tải tệp phát hành (cộng mọi tệp kể cả SHA256SUMS).
  - Còn lại: winget (👤 `WINGET_TOKEN`), P2 quảng bá (👤 đăng, agent soạn nháp khi chủ dự án yêu cầu).
