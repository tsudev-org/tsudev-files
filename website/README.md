# Website files.tsudev.com

Trang sản phẩm của TsuFiles: [Astro](https://astro.build) dựng trang tĩnh; JavaScript phía trình duyệt chỉ có
`public/theme.js` (bộ chọn giao diện Sáng/Tối/Hệ thống ở chân trang, mặc định Tối, khóa `tsudev-theme` như tsudev.com; nạp chặn trong `<head>`
để không nháy màu, CSP `script-src 'self'`, không script nội tuyến),
host trên Cloudflare Workers Static Assets (Worker `TsuFiles`, không có mã chạy; tên miền con `files.tsudev.com` của
hệ tsudev.com, cấu hình ở `wrangler.jsonc`).

* Tài liệu lấy trực tiếp từ tệp của repo (`SECURITY.md`, `docs/vi/KIEN-TRUC-BAO-MAT.md`, `CHANGELOG.md`, `LICENSE`,
  `THIRD-PARTY-NOTICES.md`) - sửa ở đó, không sửa trong `website/`. Trang Hướng dẫn, Định dạng, Dòng lệnh viết tay
  trong `src/pages/`; đổi tùy chọn dòng lệnh (`src/TsuFiles.Cli`) thì cập nhật `src/pages/dong-lenh.astro`.
* Bản phát hành (phiên bản, liên kết, SHA-256) lấy từ GitHub Releases lúc build; repo chưa có bản phát hành thì trang
  Tải về hiện thông báo "đang chuẩn bị" thay vì danh sách gói.
* Ảnh chụp `public/anh/page-*-vi.png` sinh từ test giao diện (giao diện tiếng Việt thật, không cắt tay):

  ```bash
  dotnet test tests/TsuFiles.App.Tests --filter "FullyQualifiedName~RenderTests.Main_window_pages_render"
  for f in home toolbar presets watch engines; do
    cp tests/TsuFiles.App.Tests/bin/Debug/net10.0/screenshots/page-$f-vi.png website/public/anh/
  done
  ```

* Tài sản thương hiệu (`public/favicon*`, `android-chrome-*`, `apple-touch-icon.png`, `og-image.png`,
  `brand/tsufiles-mark.*`) sinh bằng `tools/thuong-hieu/tao-tai-san.py` ở gốc repo - xem `assets/brand/README.md`;
  không sửa tay. Riêng `brand/logo-mark.png` (cú tsudev, chân trang) lấy từ dây chuyền của repo `tsudev-org/tsudev`.
* Kênh ủng hộ ở trang `/ung-ho/`: sửa `src/data/ung-ho.json` (build từ chối URL không https hoặc trỏ về dự án gốc).
* Font Be Vietnam Pro tự host (SIL OFL 1.1), token màu sao nguyên văn từ `.standards/tokens/tokens.css`.
* Header bảo mật (HSTS, CSP chặt, `X-Frame-Options: DENY`...) ở `public/_headers`; `security.txt` ở `public/.well-known/`.
  `scripts/check-dist.mjs` chạy sau mỗi lần build: title, `og:site_name`, canonical, độ dài mô tả, chặn CSS/JS nội
  tuyến, `rel` của liên kết ngoài, em-dash, liên kết nội bộ gãy.

```bash
cd website
npm ci
npm run dev          # xem thử ở http://localhost:4321
npm run build        # dựng vào dist/ và kiểm tra
npm run check:a11y   # axe-core WCAG 2.2 AA mọi trang, Sáng + Tối (lần đầu: npx playwright install chromium)
npm run check:theme  # nút đổi giao diện trong Chromium, với CSP của production
npm run deploy       # build rồi `wrangler deploy` (cần `npx wrangler login` với tài khoản tsudev)
```

Sau mỗi lần phát hành bản mới, chạy lại `npm run deploy` để trang Tải về cập nhật phiên bản và mã SHA-256.

## Thiết lập một lần

Lần đầu chỉ cần `npm run deploy`: khối `routes` trong `wrangler.jsonc` tự gắn `files.tsudev.com` (Cloudflare tạo bản
ghi DNS và chứng chỉ, khoảng 1 phút). Rồi đổi trang chủ của repo:

```bash
gh repo edit tsudev-org/tsufiles --homepage https://files.tsudev.com
```

Không dùng Cloudflare Pages: `wrangler pages project create` bản 4.158 tự chuyển sang Workers rồi lỗi, và tên miền riêng
của Pages cần bản ghi CNAME mà phiên `wrangler login` không có quyền ghi DNS. Zone tsudev.com tự chèn beacon Cloudflare
Web Analytics; CSP chặn nó nên console có một lỗi, trang vẫn chạy bình thường.

## Ủng hộ: thêm/sửa kênh

Trang `/ung-ho/` dựng từ `src/data/ung-ho.json`. Mỗi kênh là một phần tử của `"kenh"`:

| Trường | Bắt buộc | Ví dụ |
| --- | --- | --- |
| `ten` | có | `"Chuyển khoản ngân hàng"` |
| `moTa` | có | `"Quét mã VietQR bằng ứng dụng ngân hàng bất kỳ"` |
| `url` | một trong ba | `"https://github.com/sponsors/tsudev-org"` (chỉ `https://`) |
| `chiTiet` | một trong ba | `["Ngân hàng: ...", "Số tài khoản: ...", "Chủ tài khoản: ..."]` |
| `anhQr` | một trong ba | `"/ung-ho/vietqr.png"` - đặt tệp ở `public/ung-ho/vietqr.png` (tên chữ thường, số, gạch ngang) |
| `anhQrSha256` | khi có `anhQr` | `sha256sum public/ung-ho/<tệp>` - build dừng nếu ảnh bị tráo |
| `vietqr` | nên có với QR ngân hàng | `{ "bin", "soTaiKhoan", "payload" }` - build kiểm CRC, NAPAS, VND và số tài khoản trùng `chiTiet` |

Mã VietQR: KHÔNG đăng ảnh chụp từ app ngân hàng - sinh SVG vector bằng `tools/ung-ho/tao-vietqr.py --bin <BIN> --tai-khoan <số> --ra public/ung-ho/<tệp>.svg --anh-ngan-hang <ảnh chụp>` (đối chiếu từng byte với mã của ngân hàng, in `payload` và `sha256` để chép vào JSON).

Thứ tự trong danh sách là thứ tự hiển thị. `npm run build` dừng kèm lý do nếu thiếu trường, URL không phải https,
ảnh QR không có, hoặc còn trỏ về dự án gốc. Mọi thứ trong tệp này là CÔNG KHAI - không ghi mật khẩu, mã OTP, số thẻ.
Xong thì build, gộp PR, rồi `npm run deploy`. Ứng dụng không cần phát hành lại.
