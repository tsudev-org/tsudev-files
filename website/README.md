# Website files.tsudev.com

Trang sản phẩm của tsudev-files: [Astro](https://astro.build) dựng trang tĩnh; JavaScript phía trình duyệt chỉ có
`public/theme.js` (bộ chọn giao diện Sáng/Tối/Hệ thống ở chân trang, mặc định Tối, khóa `tsudev-theme` như tsudev.com; nạp chặn trong `<head>`
để không nháy màu, CSP `script-src 'self'`, không script nội tuyến),
host trên Cloudflare Workers Static Assets (Worker `tsudev-files`, không có mã chạy; tên miền con `files.tsudev.com` của
hệ tsudev.com, cấu hình ở `wrangler.jsonc`).

* Tài liệu lấy trực tiếp từ tệp của repo (`SECURITY.md`, `docs/vi/KIEN-TRUC-BAO-MAT.md`, `CHANGELOG.md`, `LICENSE`,
  `THIRD-PARTY-NOTICES.md`) - sửa ở đó, không sửa trong `website/`. Trang Hướng dẫn, Định dạng, Dòng lệnh viết tay
  trong `src/pages/`; đổi tùy chọn dòng lệnh (`src/Filee.Cli`) thì cập nhật `src/pages/dong-lenh.astro`.
* Bản phát hành (phiên bản, liên kết, SHA-256) lấy từ GitHub Releases lúc build; repo chưa có bản phát hành thì trang
  Tải về hiện thông báo "đang chuẩn bị" thay vì danh sách gói.
* Ảnh chụp `public/anh/page-*-vi.png` sinh từ test giao diện (giao diện tiếng Việt thật, không cắt tay):

  ```bash
  dotnet test tests/Filee.App.Tests --filter "FullyQualifiedName~RenderTests.Main_window_pages_render"
  for f in home toolbar presets watch engines; do
    cp tests/Filee.App.Tests/bin/Debug/net10.0/screenshots/page-$f-vi.png website/public/anh/
  done
  ```

* Tài sản thương hiệu (`public/favicon*`, `android-chrome-*`, `og-image.png`, `brand/`) sinh bằng
  `scripts/brand_assets.py` từ dây chuyền thương hiệu của repo `tsudev-org/tsudev`; không sửa tay.
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
gh repo edit tsudev-org/tsudev-files --homepage https://files.tsudev.com
```

Không dùng Cloudflare Pages: `wrangler pages project create` bản 4.158 tự chuyển sang Workers rồi lỗi, và tên miền riêng
của Pages cần bản ghi CNAME mà phiên `wrangler login` không có quyền ghi DNS. Zone tsudev.com tự chèn beacon Cloudflare
Web Analytics; CSP chặn nó nên console có một lỗi, trang vẫn chạy bình thường.
