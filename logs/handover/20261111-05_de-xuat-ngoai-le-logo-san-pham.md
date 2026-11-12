# ĐỀ XUẤT - Ngoại lệ BRAND_ASSETS mục 12.3/13: dấu hiệu riêng cho sản phẩm tsudev-files

- **Mã phiếu**: 20261111-05
- **Từ**: phiên tsudev-files-01 (11/11/2026) - **Đến**: phiên phụ trách repo `tsudev-org/tsudev-standards`
- **Trạng thái**: CHỜ NHẬN (đề xuất đổi quy ước - mở ở `proposals/` của repo quy ước theo `.standards/docs/SYNC.md` mục 1)

## 1. Bối cảnh

Ngày 11/11/2026 chủ dự án (tsudev-tsudev) quyết định dùng logo riêng `logo-tsudev-files.png` (dải xoắn xanh-vàng) làm
biểu tượng/icon/favicon/logo của toàn bộ tsudev-files: app desktop, Setup, CLI, gói Linux/macOS, website
files.tsudev.com. Quyết định đã ghi ở `logs/STATE.md` mục "Quyết định quan trọng" (đúng yêu cầu mục 13).

Quy ước hiện hành (v3.x):

- Mục 12.3: "`MUST NOT` mỗi sản phẩm tự vẽ một logo riêng"; sản phẩm chỉ `MAY` thêm huy hiệu phụ ở góc dấu hiệu chung.
- Mục 13: ảnh logo không khớp chuỗi băm bản gốc là biến thể, `MUST NOT` vào sản phẩm cho tới khi có quyết định ghi
  vào STATE - điều kiện này đã thỏa.

## 2. Đề xuất

Thêm ngoại lệ có kiểm soát vào mục 12.3: sản phẩm `MAY` có dấu hiệu sản phẩm riêng khi

1. chủ dự án quyết định và ghi vào `logs/STATE.md` (kèm SHA-256 ảnh gốc);
2. dấu hiệu riêng có dây chuyền sinh duy nhất (một ảnh gốc, một script) theo mục 7 và đủ bộ bắt buộc mục 12.1;
3. nhận diện hệ sinh thái vẫn hiện diện: liên kết về tsudev.com dùng dấu hiệu cú (website files.tsudev.com giữ cú ở
   chân trang), tên nhà phát hành "tsudev", màu chữ wordmark theo mục 5.

## 3. Bản cài đặt tham chiếu ở tsudev-files

- Ảnh gốc `assets/brand/source/logo-tsudev-files.png`
  (SHA-256 `4fa125994ee807234988b31f5d5493d4fcd361202e2fcc19e456e90b212eab3d`).
- Dây chuyền `tools/thuong-hieu/tao-tai-san.py` (tách nền trắng color-to-alpha, LANCZOS trên alpha nhân sẵn, `.ico`
  16..256, PWA maskable, AVIF/WebP/PNG, og-image), tự kiểm `--kiem`; test `BrandAssetsTests` canh trong CI.
- Tài liệu `assets/brand/README.md`.

## 4. Kết quả xử lý (phiên nhận điền)

-
