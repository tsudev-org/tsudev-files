# Website files.tsudev.com

Trang sản phẩm của tsudev-files: [Astro](https://astro.build) dựng trang tĩnh, **không có JavaScript phía trình duyệt**,
host trên Cloudflare Pages (dự án `tsudev-files`, tên miền con `files.tsudev.com` của hệ tsudev.com).

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
npm run check:a11y   # axe-core WCAG 2.2 AA mọi trang, sáng + tối (lần đầu: npx playwright install chromium)
npm run deploy       # build rồi đưa lên Cloudflare Pages (cần `npx wrangler login` với tài khoản tsudev)
```

Sau mỗi lần phát hành bản mới, chạy lại `npm run deploy` để trang Tải về cập nhật phiên bản và mã SHA-256.

## Thiết lập một lần (đã làm thì bỏ qua)

1. `npx wrangler pages project create tsudev-files --production-branch main`
2. `npm run deploy`
3. Cloudflare Dashboard, Workers & Pages, `tsudev-files`, Custom domains, thêm `files.tsudev.com` (zone `tsudev.com`
   cùng tài khoản nên Cloudflare tự tạo bản ghi CNAME và chứng chỉ).
4. `gh repo edit tsudev-org/tsudev-files --homepage https://files.tsudev.com`
