# ĐỀ XUẤT - Quy ước "Ủng hộ" dùng chung (SUPPORT.md + dữ liệu đồng bộ) cho tsudev-standards

- **Mã phiếu**: 20261112-02
- **Từ**: phiên tsudev-files-01 (12/11/2026), theo yêu cầu của chủ dự án
- **Đến**: phiên phụ trách repo `tsudev-org/tsudev-standards` (mở ở `proposals/` theo `.standards/docs/SYNC.md` mục 1)
- **Trạng thái**: CHỜ NHẬN
- **Liên quan**: phiếu gốc `~/projects/logs/handover/20261112-01_yeu-cau-dong-bo-ung-ho-he-sinh-thai.md`

## 1. Đề xuất

1. Thêm `docs/SUPPORT_AND_DONATION.md`: nội dung mục 3 của phiếu gốc thành quy tắc `MUST`/`MUST NOT` (một nguồn dữ
   liệu, app chỉ trỏ về trang, không dịch vụ QR bên thứ ba, ghim SHA-256, kiểm VietQR lúc build, CODEOWNERS, chính
   sách kho di động, không lưu bí mật, khả năng truy cập).
2. Phát hành dữ liệu dùng chung trong gói đồng bộ: `ung-ho/ung-ho.json` (kênh, `vietqr`, `anhQrSha256`) và
   `ung-ho/vietqr-vib.svg` - lấy nguyên tệp từ `tsudev-files` (SHA-256
   `7209698f1393857607ec7bea95b06349ca8ce332d6fb0c9f1774d6beab4cecae`). `MANIFEST.sha256` đã ghim mọi tệp trong gói,
   nên repo con đọc từ `.standards/ung-ho/` là có toàn vẹn sẵn; đổi tài khoản = một bản quy ước mới, đồng bộ xuống.
3. Thêm vào `scripts/check-standards.sh`: tìm URL kênh ủng hộ của dự án gốc phổ biến (`github.com/sponsors/<không
   phải tsudev>`, `ko-fi.com`, `patreon.com`, `paypal.me`, `buymeacoffee.com`, `fairy.hada.io`) trong mã/tài liệu của
   repo con -> LƯU Ý (repo kế thừa có thể miễn theo dòng như `.standards-upstream`).
4. `ECOSYSTEM_IDENTITY.md`: thêm "Ủng hộ" vào chân trang chung, trỏ `https://tsudev.com/ung-ho/`.

## 2. Bản cài đặt tham chiếu

`tsudev-files`: `website/src/data/ung-ho.json`, `website/src/lib/ung-ho.ts` (`kiemVietQr`, `crc16`, ghim SHA-256),
`website/src/pages/ung-ho.astro`, `tools/ung-ho/tao-vietqr.py`, test `SponsorTests` (app chỉ trỏ về tsudev).

## 3. Kết quả xử lý (phiên nhận điền)

-
