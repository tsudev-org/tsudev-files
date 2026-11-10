# PHIẾU YÊU CẦU - Nút đổi giao diện Sáng/Tối trong header website files.tsudev.com

- **Mã phiếu**: 20261110-02
- **Từ**: phiên tsudev-c8 (repo `tsudev`) - **Đến**: phiên Claude của repo `tsudev-files`
- **Thời điểm**: 10/11/2026
- **Trạng thái**: ĐÓNG (10/11/2026) - còn một việc ngoài repo cho chủ dự án, xem Kết quả xử lý
- **Ủy quyền**: chủ dự án yêu cầu bổ sung nút đổi màu sáng/tối ở header website của mọi dự án hệ tsudev
  (chat 10/11/2026, phiên tsudev-c8).

## Yêu cầu

Thêm **nút đổi giao diện Sáng/Tối** vào header của website `files.tsudev.com` (thư mục `website/ (đang ở nhánh `feat/website`, bản làm việc `~/projects/tsudev-files-website`)`), cùng hành vi với nút
trên tsudev.com để người dùng thấy một hệ sinh thái nhất quán.

1. **Nút trong header**, cạnh điều hướng chính, cả desktop lẫn di động. Biểu tượng mặt trời/mặt trăng đổi theo
   chế độ đang hiện. Bấm mở menu nhỏ 4 lựa chọn: **Sáng · Ấm · Tối · Theo hệ thống** (giống
   `packages/ui/src/components/ThemeToggle.tsx` của repo tsudev). Nếu muốn gọn: nút bật/tắt Sáng ↔ Tối cũng đạt,
   nhưng phải giữ được lựa chọn "Theo hệ thống".
2. **Ghi chế độ vào `data-theme` trên `<html>`** - `tokens.css` của site đã có đủ khối `light`/`warm`/`dark`.
   Lưu lựa chọn ở `localStorage` khoá **`tsudev-theme`**, giá trị `light|warm|dark|system` (cùng khoá với
   tsudev.com). `system` được GIẢI bằng `matchMedia('(prefers-color-scheme: dark)')` trong JavaScript - đừng
   treo bảng màu vào `@media (prefers-color-scheme)` trong CSS.
3. **Chống nháy màu**: áp chế độ đã lưu bằng script ĐỒNG BỘ trong `<head>`, TRƯỚC stylesheet - không dùng
   `defer`/`type=module`/`DOMContentLoaded` (cho ra một khung hình sai màu ở mọi lần tải). Bọc `try/catch`
   quanh `localStorage` (ném lỗi khi chặn cookie/iframe sandbox) - hỏng thì về chế độ mặc định, không trang trắng.
   Đồng thời cập nhật `<meta name="theme-color">` theo `--bg-base` của chế độ đang hiện.
4. **CSP - đọc kỹ, đây là chỗ dễ hỏng im lặng.** `website/public/_headers` hiện khai `default-src 'none'` và **KHÔNG có `script-src`** - tức là site không cho chạy BẤT KỲ script nào; một nút đổi giao diện viết thêm vào sẽ chết im lặng ở production. Cách làm: đặt script vào tệp tĩnh (vd. `public/theme.js`), nạp bằng `<script src="/theme.js"></script>` (KHÔNG `defer`/`async` - phải chặn để áp màu trước khi vẽ), và thêm `script-src 'self'` vào CSP. Không dùng `'unsafe-inline'`.
5. **Khả năng truy cập**: `<button type="button">` có `aria-label="Đổi giao diện"`, `aria-haspopup`/`aria-expanded`
   cho menu, Esc đóng menu, vòng focus nhìn thấy được; mục đang chọn có `aria-checked`/`aria-current`.
6. **Tương phản**: duyệt MỌI trang ở cả 3 chế độ - `site.css` có thể còn mã hex cắm cứng chỉ đúng ở chế độ Sáng.
   Màu phải đi qua token để đổi theo `data-theme`.
7. **Chế độ mặc định giữ nguyên** như hiện tại của site (`data-theme` đang đặt cứng trong `Trang.astro`), trừ khi
   chủ dự án quyết khác. Ghi chú: từ 10/11/2026 tsudev.com mặc định Tối (nền gần-đen `#05070B`, xanh dương
   `#3D9BFF` + xanh lá `#2EE6A6`) - nếu chủ dự án muốn đồng bộ thì đó là một quyết định riêng, đừng tự đổi.

## Nghiệm thu

- Build sạch; nếu repo có test quét nguồn thì thêm test: script chống nháy nằm trong `<head>`, giải đủ 4 lựa chọn,
  và CSP cho phép đúng cách nạp script đó.
- Sau deploy: `curl -sI https://files.tsudev.com/ | grep -i content-security-policy` khớp cách nạp script; mở trang bằng
  trình duyệt thật, đổi qua 4 lựa chọn, tải lại trang - chế độ được giữ và **không có cú nháy**; console không có
  lỗi CSP (`Refused to execute inline script`).
- Ghi kết quả vào mục "Kết quả xử lý" của phiếu này, rồi đổi Trạng thái thành ĐÓNG.

## Tham chiếu

- Repo tsudev (`~/projects/tsudev` hoặc `github.com/tsudev-org/tsudev`): `apps/frontend-main/pages/_document.tsx`
  (`THEME_SCRIPT`), `packages/ui/src/components/ThemeToggle.tsx`, `apps/frontend-main/test/themeTokens.test.ts`.
- Bài học đã trả giá ở tsudev: đổi nội dung script nội tuyến mà quên cập nhật băm CSP ⇒ production chặn script,
  site vẫn 200, không gì báo lỗi. Dev/E2E chạy `http://localhost` không có CSP nên không bắt được.

## Kết quả xử lý

Phiên tsudev-files, 10/11/2026. **PR #25** gộp (`ed49cbfc`); chủ dự án deploy (Worker version `5c375f4d`).

- Cách làm: `website/public/theme.js` nạp chặn bằng `<script src="/theme.js">` ngay sau các meta, trước stylesheet;
  CSP thêm `script-src 'self'` (không `'unsafe-inline'`). Mặc định `system` (giữ hành vi cũ của site, không tự đổi
  sang Tối). Menu 4 lựa chọn ARIA `menuitemradio`; không JavaScript thì nút ẩn và trang vẫn theo hệ điều hành.
- Test quét nguồn/trình duyệt (chạy trong CI `website.yml`): `check-dist` (đúng một script, trong `<head>`, trước
  CSS; CSP có `script-src 'self'`, không `unsafe-*`), `check-theme` (Chromium với CSP production: 4 lựa chọn, giữ sau
  tải lại, `data-theme` có trước `<body>`, bàn phím, localStorage bị chặn, giá trị lạ, không JS, 320/390/1280px),
  `check-a11y` (axe 10 trang x Sáng/Ấm/Tối + menu mở: 0 lỗi). Đột biến: bỏ `script-src` hoặc thêm `defer` đều bị bắt.
- Nghiệm thu production (Chromium thật, `https://files.tsudev.com/`):
  - `curl -sI` -> `content-security-policy: default-src 'none'; script-src 'self'; ...`; `/theme.js` 200 `text/javascript`.
  - Mặc định: `data-theme=light`, chọn `system`. Bấm Sáng/Ấm/Tối/Theo hệ thống rồi tải lại: giữ đúng
    (`localStorage tsudev-theme` = giá trị đã chọn), `data-theme` đã đúng lúc `<body>` bắt đầu (không nháy),
    `theme-color` và nền đổi theo (`#EEF4FB`/`#F6F1E6`/`#0F1B2D`). "Theo hệ thống" + hệ điều hành chuyển tối -> `dark` ngay.
  - Console: **không có lỗi từ `theme.js`**. Còn lỗi CSP từ script do **Cloudflare tự chèn** vào HTML (không có trong
    repo): Web Analytics `static.cloudflareinsights.com/beacon.min.js` và script nội tuyến
    `/cdn-cgi/challenge-platform` (phát hiện bot). Chúng vốn đã bị CSP chặn từ trước (khi chưa có `script-src`).
    KHÔNG nới CSP cho chúng (trang cam kết không thu thập dữ liệu). Việc cho chủ dự án: tắt tự chèn Web Analytics
    và JavaScript Detections/Bot Fight Mode cho `files.tsudev.com` trong bảng điều khiển Cloudflare (thiết lập zone).
