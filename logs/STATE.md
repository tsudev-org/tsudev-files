# STATE.md - Trạng thái project tsudev-files

> Agent đọc file này ở đầu mỗi phiên và cập nhật ở cuối mỗi phiên.
> Quy trình: `.standards/docs/AGENT_PROTOCOL.md`.

## Hàng đợi task (làm từ trên xuống)

- [ ] (Chủ dự án quyết) Lỗi console từ script nội tuyến `/cdn-cgi/challenge-platform` mà Cloudflare chèn (JavaScript
      Detections; Web Analytics và Bot Fight Mode đã tắt 10/11). CSP chặn nên trang không ảnh hưởng. JavaScript
      Detections là thiết lập CẤP ZONE: tắt (`PUT /zones/<id>/bot_management {"enable_js": false}`, token Zone > Bot
      Management: Edit) ảnh hưởng MỌI site *.tsudev.com. tsudev.com không bị lỗi vì CSP dùng nonce (Cloudflare gắn
      nonce vào script challenge); site tĩnh này không sinh nonce được. Lựa chọn: giữ nguyên (chỉ là lỗi console) hoặc
      tắt cả zone. Không nới CSP.
- [ ] Phiếu `logs/handover/20261110-01_*.md`: áp dụng miễn trừ em-dash theo dòng khi `tsudev-standards` PR #48 gộp và có
      nhãn v3.3.0. Gồm sửa vi phạm THẬT của fork: `src/Filee.App/Assets/i18n/ru.json` khóa `installer.error.incomplete`
      (dòng fork sửa URL, câu gốc có em-dash -> thay bằng `-`). (Regex `check-dist.mjs` đã sửa ở PR #25.)
- [ ] Gộp PR #28 (hộp Giới thiệu liên kết tsudev.com + files.tsudev.com, Company/Copyright tsudev; ECOSYSTEM_IDENTITY
      2.3) khi CI xanh.
- [ ] Sao lưu các tệp build đang tải từ `dl.filee.sh` (máy chủ upstream: engine Windows ARM, Magick.Native macOS, font -
      `build/fetch-engines.ps1`, `build/fetch-magick-native.ps1`, `build/fetch-fonts.ps1`) sang nơi 0 đồng của tsudev-org,
      giữ nguyên SHA-256 ghim. KHÔNG đặt vào Releases của repo này (trình cập nhật đọc releases/latest) - dùng repo
      riêng trong `tsudev-org` hoặc Cloudflare R2/Pages. Cần chủ dự án duyệt nơi lưu.
- [ ] Phát hành bản đầu tiên `v2.4.0-vn.1` theo `docs/vi/PHAT-HANH.md` (cần máy Windows có PowerShell 7 để ký SHA256SUMS).
- [ ] Mua/xin chứng thư Authenticode (hoặc SignPath OSS) rồi điền `FileeExpectedPublisher`/`FileeExpectedIssuer`.
- [ ] Chủ dự án mở Issue/PR ở `tsudev-standards` theo bản nháp `logs/handover/20261109-01_de-xuat-mien-tru-emdash-upstream.md`.

## Đang thực hiện

| Task | Agent | Bắt đầu |
| --- | --- | --- |

## Đã hoàn thành (mới nhất trên cùng)

- 10/11/2026 - Giao diện website đen tuyền + tím kiểu Supabase, bộ chọn Sáng/Tối/Hệ thống ở chân trang (phiếu
  `20261110-05`, PR #29, deploy `b57f2ec3`, nghiệm thu production). Ruleset "Bảo vệ main" (id 26937532: chặn
  force-push/xóa, bắt buộc PR + check `build`, `Quét lộ bí mật (gitleaks)`, admin chỉ bỏ qua khi gộp PR). Dependabot:
  13 PR lẻ tự đóng khi chuyển sang nhóm; gộp #18 (actions, SHA đã đối chiếu tag) và #22 (playwright); đóng #24 (nhóm
  NuGet có bản major SkiaSharp 3->4, xunit.v3 3->4 làm đỏ mọi job) và gộp #27 (bỏ qua bản major của NuGet).

- 10/11/2026 - Phiếu `20261109-05` mục 2: gộp PR #10 (đổi tên hiển thị + sửa tên ở thanh bên/Giới thiệu/Setup, kiểm
  lối tắt bộ cài, `ForkDefaults.Brand` không regex - hết sập `filee help engines`), PR #21 (URL `tsudev-org`, quy ước
  v3.2.0, README). Website `files.tsudev.com` (PR #20, Astro, Cloudflare Workers Static Assets) đã chạy trên production.
  Đóng phiếu 03, 04, 05. PR #25: nút đổi giao diện Sáng/Ấm/Tối/Theo hệ thống (phiếu `20261110-02`), đã deploy và
  nghiệm thu production.

- 09/11/2026 - Chuyển repo sang `tsudev-org` + Public (đã bật lại secret scanning/push protection); `tsudev-contact`, `app-la-ban` Public; gộp PR #16 (Dependabot hằng tháng), #17 (gitleaks binary); nhận diện tsudev.com cho 8 repo. Chi tiết: phiếu `20261109-05`.

- 09/11/2026 - Gộp PR #1 (khởi tạo fork); xác minh 2 test App chập chờn; mở PR #10 (đổi tên) và PR #15 (gitleaks binary ghim SHA-256, `.gitleaksignore` 6 dương tính giả upstream).

- 09/11/2026 - Đổi tên hiển thị "Filee" -> "tsudev-files" (ADR-0001): 6 commit trên `feat/doi-ten-hien-thi`; test Core/Cli/Platform đạt, App 2221/2242 đạt (2 lỗi thời gian nghi chập chờn, xem phiếu `20261109-04`). Thêm quyền `pull-requests: read` cho gitleaks (`fccc2d32`).

- 09/11/2026 - Việt hóa thông báo installer macOS; thêm `installer/{linux,macos}/HUONG-DAN-CAI-DAT.txt` và đóng gói kèm trong `build/build-portable.ps1`.
- 09/11/2026 - Soạn đề xuất miễn trừ em-dash cho mã kế thừa: `logs/handover/20261109-01_de-xuat-mien-tru-emdash-upstream.md`.
- 09/11/2026 - Việt hóa installer Linux (`say "<vi>" "<en>"`, `FILEE_LANG=en` ra tiếng Anh) và tiêu đề dự phòng menu Explorer; test `ForkLinuxInstallerTests` (commit `52ca56f0`).
- 09/11/2026 - Kiểm chứng và commit phần khởi tạo fork thành 5 commit trên nhánh `chore/khoi-tao-fork` (phiên trước tắt khi chưa commit). Kết quả test: phiếu `20261109-02`.
- 09/11/2026 - Khởi tạo fork từ `KnifeLemon/Filee@64890f1e` (v2.4.0): rà soát an toàn, tiếng Việt mặc định (GUI, Setup, CLI), trình cập nhật trỏ về repo riêng + khóa ký riêng, ghim action theo SHA, thêm CodeQL/gitleaks/Dependabot/kiểm tra bản dịch.

## Quyết định quan trọng

- 10/11/2026 - Website `files.tsudev.com` (thư mục `website/`): Astro trang tĩnh, host Cloudflare Workers Static Assets
  (Worker `tsudev-files`, `custom_domain` trong `wrangler.jsonc`) thay vì Pages - OAuth của `wrangler login` không ghi
  được DNS cho CNAME của Pages. Deploy: chủ dự án chạy `cd website && npm run deploy`. JavaScript phía trình duyệt chỉ
  có `public/theme.js` (CSP `script-src 'self'`, không script nội tuyến). Giao diện: từ 10/11/2026 theo quyết định chung
  của hệ sinh thái (phiếu `20261110-05`): đen tuyền + tím, chỉ Sáng/Tối/Hệ thống, mặc định Tối.
- 10/11/2026 - Dependabot NuGet bỏ qua bản major (PR #27): bản major chờ upstream KnifeLemon/Filee nâng rồi gộp về.
- 10/11/2026 - Giữ tên lệnh `filee` (không đổi thành `tsudev-files`): script ngoài phụ thuộc, ADR-0001. Muốn có tên
  phụ `tsudev-files` thì cần ADR riêng.

- 09/11/2026 - Spending limit GitHub Actions giữ 0 USD; Dependabot chạy hằng tháng và gom nhóm (PR #16) để tiết kiệm phút.

- 09/11/2026 - Tên hiển thị là `tsudev-files`, định danh nội bộ giữ `Filee` (ADR-0001, `docs/adr/0001-doi-ten-hien-thi-tsudev-files.md`); tệp phát hành đổi thành `tsudev-files-<ver>-...`, trình cập nhật nhận cả hai tiền tố.
- 09/11/2026 - Giữ tên namespace/assembly `Filee.*` để gộp upstream dễ; mọi khác biệt của fork gom vào `src/Filee.Core/ForkDefaults.cs` + `Directory.Build.props`.
- 09/11/2026 - Không sửa em-dash trong mã kế thừa từ upstream (48 file, gồm dữ liệu test cố ý chứa ký tự ngoài Latin-1): sửa sẽ gây xung đột khi gộp và làm yếu test. Mã MỚI của fork tuân thủ mục 7.1. `check-standards.sh` vì vậy còn báo TRƯỢT ở mục 4 cho tới khi trung tâm có cơ chế miễn trừ.
- 09/11/2026 - Khóa ký cập nhật ECDSA P-256 riêng; khóa bí mật ở `~/.config/tsudev-files/keys/update-key.pem` (0600, ngoài repo). Bản phát hành của upstream KHÔNG được bản fork chấp nhận.

- 09/11/2026 - Giữ nút tài trợ trỏ về tác giả gốc KnifeLemon (`SponsorLinks.cs`, `.github/FUNDING.yml`, README): ghi công
  upstream, tránh xung đột khi gộp. Đổi thì hỏi chủ dự án.
- 09/11/2026 - Repo chuyển sang `tsudev-org` và thành Public (Actions miễn phí, CodeQL, Secret Scanning, runner Windows ARM).
  Hạng sản phẩm: C - Ngoại tuyến.

## Sự cố bảo mật

- (chưa có)
