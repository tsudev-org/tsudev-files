# STATE.md - Trạng thái project tsudev-files

> Agent đọc file này ở đầu mỗi phiên và cập nhật ở cuối mỗi phiên.
> Quy trình: `.standards/docs/AGENT_PROTOCOL.md`.

## Hàng đợi task (làm từ trên xuống)

- [ ] ⚡ TIẾP THEO: phiếu `logs/handover/20261113-03_ban-giao-tsufiles-t3b.md` - TsuFiles (ADR-0003): T3b-1..3 đã gộp
      (#57-#59: thư mục dữ liệu, lệnh `tsufiles`, tệp chạy `TsuFiles`); tiếp T3b-4 (GNOME/KWin/WM_CLASS, bundle macOS,
      tên registry/MSIX) rồi phát hành 2.4.1. ĐỌC ADR-0003 mục 5 (ràng buộc trình cập nhật 2.4.0-vn.1).

- [ ] Kế hoạch gia cố + hard fork (11/11/2026), mỗi phase một PR, làm theo thứ tự:
  - P1 PR #42 tải tệp build từ `tsudev-org/tsudev-files-assets` - CHỜ chủ dự án tạo repo assets (job win-arm64 đỏ tới lúc đó).
  - P2 PR #43 SBOM CycloneDX + chứng thực - ĐÃ GỘP 12/11/2026 (`70768d88`).
  - P3 PR #44 (nháp) vòng khóa ký - CHỜ chủ dự án tạo khóa dự phòng.
  - P4 PR #45 fuzz bộ đọc tự viết - ĐÃ GỘP 12/11/2026 (`4e2dfd8a`).
  - P5a đổi tên mã nguồn `Filee.*` -> `Tsudev.Files.*` (ADR-0002) bằng `tools/doi-ten-tsudev.py`, nhánh
    `feat/p5a-doi-ten-ma-nguon` - PR #46 (đã dựng lại trên `main` sau P2/P4; CI đỏ thứ tự using, phiếu 20261112-03). P1/P3 gộp sau
    thì rebase rồi chạy lại script. Sửa tay chỉ 4 tệp: AGENTS.md, CONTRIBUTING.md, LICENSE, ADR-0002.
  - P5b định danh lúc chạy + di trú bản đã cài (ADR-0002 mục 3) - bản 2.4.1.
  - **P6 thương hiệu + Ủng hộ** (yêu cầu chủ dự án 11/11/2026), PR #47 trên nhánh `feat/p6-thuong-hieu-va-ung-ho` xếp sau P5a:
    logo riêng `assets/brand/source/logo-tsudev-files.png` + dây chuyền `tools/thuong-hieu/tao-tai-san.py` (thay
    `filee.svg`, `website/scripts/brand_assets.py`, `DonutLogo` vẽ bằng mã -> `AppLogo`); trang `/ung-ho/` (dữ liệu
    `website/src/data/ung-ho.json`) là nơi duy nhất liệt kê kênh ủng hộ - app, README, `FUNDING.yml` trỏ về đó; bỏ
    README dịch của upstream (`docs/readme/`) và ảnh quảng bá Filee (`docs/media/`). Chỉ áp cho tsudev-files (lotus
    là dự án riêng - phiếu gửi nhầm đã rút, đã nhắn `tsudev-lotus-0e`).
  - 12/11/2026: kênh duy nhất VIB `056485506` (NGUYEN TRANG TINH SU). QR sinh lại dạng SVG từ chuỗi VietQR đã đối chiếu
    từng byte với mã của ứng dụng VIB (`tools/ung-ho/tao-vietqr.py`); build kiểm CRC/BIN/số tài khoản + ghim SHA-256;
    CODEOWNERS riêng cho tệp ủng hộ. Phiếu đồng bộ cho cả hệ sinh thái: `~/projects/logs/handover/20261112-01_*`
    (+ phiếu con ở tsudev, lotus, trafficmonitor, cwico, swico, contact; đề xuất quy ước `20261112-02`).
  - Sau khi gộp PR P6: chủ dự án deploy website (`cd website && npm run deploy`).
  - Còn tài nguyên upstream, xử lý ở phase sau: `dl.filee.sh` (P1 #42), tiện ích GNOME `filee-donut@filee.sh`, chữ ký
    Authenticode `/du filee.sh` (P1), `build/sign-release.ps1` + CONTRIBUTING (gương R2 của upstream), Help CLI "bản
    Việt hóa của KnifeLemon/Filee" (P5b); liên kết issue upstream trong `docs/FORMAT-RULES.md` là trích dẫn lịch sử.
  - (Xong 11/11/2026) Chủ dự án đã tải `website/public/og-image.png` làm Social preview của repo GitHub; đổi logo thì
    tải lại ảnh này (chỉ làm được qua giao diện web).

- [ ] Dọn nhánh đã gộp trên `origin` (agent bị chặn xóa nhánh từ xa): chủ dự án chạy lệnh trong phiếu
      `20261111-01` mục 2.1 (+ `feat/packaging-package-json` sau khi gộp) và bật "Automatically delete head branches".
- [ ] Sao lưu các tệp build đang tải từ `dl.filee.sh` (máy chủ upstream: engine Windows ARM, Magick.Native macOS, font -
      `build/fetch-engines.ps1`, `build/fetch-magick-native.ps1`, `build/fetch-fonts.ps1`) sang nơi 0 đồng của tsudev-org,
      giữ nguyên SHA-256 ghim. KHÔNG đặt vào Releases của repo này (trình cập nhật đọc releases/latest) - dùng repo
      riêng trong `tsudev-org` hoặc Cloudflare R2/Pages. Cần chủ dự án duyệt nơi lưu.
- [ ] Gói Windows ARM64 chưa có trong bản phát hành (`release.yml` chỉ build win-x64); upstream đang thêm ở
      KnifeLemon/Filee PR #1339 - lấy về khi gộp upstream.
- [ ] Mua/xin chứng thư Authenticode (hoặc SignPath OSS) rồi điền `FileeExpectedPublisher`/`FileeExpectedIssuer`.

- [ ] Tệp `tsudev-files-<ver>-SHA256SUMS.txt` chính chỉ liệt kê tệp Windows (Linux/macOS ở tệp riêng theo nền tảng);
      PACKAGING.md mục 4.1 muốn một SHA256SUMS liệt kê mọi tệp. Không đổi tên/định dạng tệp hiện có vì trình cập nhật
      đọc chúng - nếu làm thì THÊM tệp gộp mới ở bản 2.4.1 và ghi vào `docs/vi/PHAT-HANH.md`.

## Đang thực hiện

| Task | Agent | Bắt đầu |
| --- | --- | --- |

## Đã hoàn thành (mới nhất trên cùng)

- 13/11/2026 - ADR-0003 T3b-1..3 gộp: #57 thư mục dữ liệu `TsuFiles` + di trú `LegacyDataMigration`, biến `TSUFILES_*`;
  #58 lệnh `tsufiles`/`tsufiles-cli`; #59 tệp chạy `TsuFiles(.exe)`, Setup tiếp quản `Filee.exe`, launcher `Filee` trong
  gói Linux. `main` @ `9c11f17d`. Phiếu `20261113-03`.

- 13/11/2026 - Gói APT/RPM tsudev-files 2.4.0-vn.1 xuất bản trên `pkg.tsudev.com` (linux-packages PR #13, cài thật xanh
  8 môi trường); `apt`, `rpm` chuyển sang `channels`; khối "Cài bằng lệnh" ở `/tai-ve/`. Phụ thuộc Linux: PR #53.

- 13/11/2026 - ADR-0003 T2 gộp PR #49 (`290456b4`): `Tsudev.Files.*` -> `TsuFiles.*`, `TsuFiles.slnx`, tên hiển thị
  TsuFiles, kiểu giao diện Cú đêm (`ThemeStyle.NightOwl`, mặc định bản cài mới + Tối), logo `tsufiles-mark.*`, og-image.
  T3a bắt buộc cập nhật: PR #50 (`VersionGate`, `ForcedUpdateService`, `website/public/phien-ban.json`). Phiếu: `20261113-01`.

- 12/11/2026 - Phiếu `20261112-03` mục 2.1-2.2: sửa thứ tự `using` (`2d02d52b`), gộp PR #46 (P5a) và #47 (P6, thêm
  logo cú TsuFiles - T1 của ADR-0003), `main` @ `39af4028`. CodeQL báo 4 `cs/cleartext-storage` ở `Program.cs` là cảnh
  báo cũ của `main` (đổi đường dẫn), dương tính giả (ghi log `IntegrityLevel`). Chủ dự án chốt tên TsuFiles, lệnh
  `tsufiles` không bí danh, namespace `TsuFiles.*`, đổi tên repo ở phase cuối (ADR-0003).

- 12/11/2026 - Gộp #45 (P4 fuzz), #43 (P2 SBOM). Mở #46 (P5a), #47 (P6 logo + Ủng hộ VietQR VIB). Phiếu Ủng hộ cho
  cả hệ sinh thái + phiếu lựa chọn theo nền tảng (`~/projects/logs/handover/20261112-0{1,2}_*`). Bàn giao: `20261112-03`.
- 11/11/2026 - Phiếu `20261111-02`: thêm `packaging/package.json` (schema 1, khớp tên tệp Release `v2.4.0-vn.1`;
  `revision` = `vn.1`); gửi phiếu sửa bộ dựng cho `linux-packages`. Phiếu `20261111-03` (tài khoản gh): đã đọc, dùng
  mục 2. `release.yml`: thêm bước `actions/attest-build-provenance` v4.2.2 (PACKAGING.md mục 4.1) - có hiệu lực từ bản
  2.4.1; cách kiểm ở `docs/vi/PHAT-HANH.md` bước 5. PR #40 (CI 19/19 xanh) đã gộp, `3875c931`.

- 11/11/2026 - PR #36 thay PR #31 (Dependabot NuGet, đỏ mọi job): nâng Avalonia 12.1.6, Logging/Pkcs 10.0.15, Magick.NET
  15.1.0, Markdig 1.6.0, ExcelNumberFormat 1.2.0, AngleSharp 1.10.1, Tomlyn 2.11.1, MsgReader 6.2.1. CI 20/20 xanh.
  Đóng #31. Trên WSL: Engines trượt 2 (phông chữ), Setup trượt 15 (đường dẫn Windows) - lỗi môi trường, CI đạt.
- 11/11/2026 - **Phát hành `v2.4.0-vn.1`** (bản đầu tiên, latest): https://github.com/tsudev-org/tsudev-files/releases/tag/v2.4.0-vn.1
  Workflow Release `42851826849` xanh; 8 gói (Windows Setup/Portable x64, Linux x64/arm64 tar.gz, macOS arm64/x64
  dmg+tar.gz) khớp SHA256SUMS; 5 tệp SHA256SUMS ký ECDSA trên WSL (`tools/sign-checksums-vn.sh`), đã tải `.sig`.
  Chạy thử gói Linux x64: `filee-cli --version` = 2.4.0-vn.1, `help` tiếng Việt, chuyển `.txt -> .md` đạt. Gộp PR #33
  (quy ước v3.3.0, job CI "Quy ước tsudev"). Ký SHA256SUMS không còn cần máy Windows.
  Website: chủ dự án deploy (Worker version `90e83133`); production `/tai-ve/` hiện đủ 8 gói, `/nhat-ky/` có mục
  2.4.0-vn.1, liên kết tải Setup.exe trả 200.
- 10/11/2026 - Phiếu `20261110-06`: gộp PR #30, #28 (hộp Giới thiệu liên kết tsudev.com, nhà phát hành tsudev), #32
  (CHANGELOG 2.4.0-vn.1); đẩy nhãn `v2.4.0-vn.1`.
- 10/11/2026 - Quy ước v3.3.0: `.standards-upstream` (miễn em-dash kế thừa theo dòng, KnifeLemon/Filee@64890f1e), sửa
  em-dash do fork ở `ru.json`, job CI "Quy ước tsudev" (`STANDARDS_VERIFY_UPSTREAM=1`). `check-standards.sh` ĐẠT.
  Đóng phiếu `20261110-01`, `20261109-01`.

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

- 13/11/2026 - Tiền tố tệp phát hành GIỮ `tsudev-files-` và gói Linux giữ tệp chạy tên `Filee` (launcher): trình
  cập nhật của 2.4.0-vn.1 chỉ nhận như vậy (ADR-0003 mục 5). Cài mới Windows vào thư mục `TsuFiles`, cập nhật giữ thư
  mục cũ. Biến `FILEE_*` và lệnh `filee` bỏ hẳn (gãy tương thích, ghi CHANGELOG). Dữ liệu di trú một lần, không ghi đè.

- 13/11/2026 - Tên còn phụ thuộc bởi bản 2.4.0-vn.1 đã cài (lối tắt `tsudev-files.lnk`, script Nautilus, tiền tố tệp
  phát hành) gom vào `ForkDefaults.InstalledName`; chỉ đổi ở T3b cùng di trú. Trình cập nhật đã nhận cả tiền tố
  `Filee-`, `tsudev-files-`, `tsufiles-`.
- 13/11/2026 - Kiểu Cú đêm: tím logo đo được `#8840F0` không đạt 4.5:1 làm chữ trên nền tối, nên dùng `#A06BF5` (tối),
  `#6A2BD0` (sáng); màu nhấn cố định, ẩn bộ chọn màu nhấn. Bản cài mới (chưa có settings.json) = Cú đêm + Tối; test App
  dựng theo Soft (`TestApp.cs` đặt lại `Theme`).
- 13/11/2026 - Bắt buộc cập nhật so `ForkDefaults.BuildNumber` (1 = 2.4.0-vn.1, 2 = 2.4.1, +1 mỗi bản). Bản cũ còn
  dùng được không thêm hộp thoại (thẻ cập nhật sẵn có đã nhắc). Quy trình: `docs/vi/CAP-NHAT-BAT-BUOC.md`.

- 12/11/2026 - Tên sản phẩm **TsuFiles**, logo con cú, lệnh `tsufiles` (bỏ `filee` không bí danh), namespace
  `TsuFiles.*`, kiểu giao diện Cú đêm mặc định, repo đổi thành `tsudev-org/tsufiles` ở phase cuối:
  `docs/adr/0003-ten-san-pham-tsufiles.md`. Thay thế các quyết định "giữ tên lệnh `filee`" và tên đích `Tsudev.Files.*`.

- 12/11/2026 - **Ủng hộ theo nền tảng** (chủ dự án ủy quyền; đối chiếu chính sách Google Play/App Store 12/11/2026):
  web hiện đầy đủ; desktop/Android chỉ nút mở trang web ("tự nguyện, 100% tới nhà phát triển, không mở khóa gì");
  iOS App Store ẩn hoàn toàn. tsudev-files chỉ phát hành desktop -> nút mở `files.tsudev.com/ung-ho/`. Chi tiết và
  việc của các repo khác: `~/projects/logs/handover/20261112-02_quyet-dinh-ung-ho-theo-nen-tang.md`.
- 11/11/2026 - **Logo riêng của tsudev-files** (chủ dự án): mọi icon/favicon/logo sinh từ
  `assets/brand/source/logo-tsudev-files.png` (SHA-256 `4fa125994ee807234988b31f5d5493d4fcd361202e2fcc19e456e90b212eab3d`)
  bằng `tools/thuong-hieu/tao-tai-san.py`; không sửa tay tệp sinh ra (`assets/brand/README.md`). Cú tsudev chỉ còn ở
  chân trang website (liên kết hệ sinh thái). Lệch `BRAND_ASSETS.md` mục 12.3 - đã đề xuất ngoại lệ: phiếu `20261111-05`.
- 11/11/2026 - **Ủng hộ chỉ trỏ về tsudev** (chủ dự án; thay quyết định 09/11/2026 "giữ nút tài trợ KnifeLemon"): kênh
  ủng hộ khai DUY NHẤT ở `website/src/data/ung-ho.json`, hiện ở `files.tsudev.com/ung-ho/`; build website từ chối URL
  không https hoặc trỏ về dự án gốc; test `SponsorTests` chặn URL ngoài `files.tsudev.com`/`github.com/tsudev-org`.
  Ghi công tác giả gốc và giấy phép MIT giữ nguyên (nghĩa vụ giấy phép, không phải kênh ủng hộ).

- 11/11/2026 - Tài khoản `gh` active là chung cho mọi phiên trên máy (phiên b4djl1h-cmdb hay đổi qua lại). Từ
  11/11/2026 chủ dự án thêm `b4djl1h` làm Owner `tsudev-org` (đã kiểm: org admin, repo admin) - cả `tsudev-tsudev` và
  `b4djl1h` đều ghi được, đổi tài khoản không còn chặn việc. Chỉ tránh đổi đúng lúc phiên khác đang push/merge; lệnh
  ghi lỗi quyền thì chạy `gh api user -q .login` trước.
- 11/11/2026 - Thẻ phát hành của fork dạng `v<x.y.z>-vn.<n>`; trình cập nhật bỏ hậu tố, nên bản sửa riêng tiếp theo
  phải tăng số vá (2.4.1...), không chỉ tăng `-vn.<n>`.
- 10/11/2026 - Website `files.tsudev.com` (thư mục `website/`): Astro trang tĩnh, host Cloudflare Workers Static Assets
  (Worker `tsudev-files`, `custom_domain` trong `wrangler.jsonc`) thay vì Pages - OAuth của `wrangler login` không ghi
  được DNS cho CNAME của Pages. Deploy: chủ dự án chạy `cd website && npm run deploy`. JavaScript phía trình duyệt chỉ
  có `public/theme.js` (CSP `script-src 'self'`, không script nội tuyến). Giao diện: từ 10/11/2026 theo quyết định chung
  của hệ sinh thái (phiếu `20261110-05`): đen tuyền + tím, chỉ Sáng/Tối/Hệ thống, mặc định Tối.
- 10/11/2026 - KHÔNG tắt JavaScript Detections của Cloudflare (thiết lập cấp zone, ảnh hưởng mọi site *.tsudev.com;
  Bot Fight Mode cần nó nếu bật lại). Hệ quả chấp nhận: console files.tsudev.com báo CSP chặn script nội tuyến
  `/cdn-cgi/challenge-platform` do Cloudflare chèn; trang không ảnh hưởng. Không nới CSP. Hết lỗi khi site có phần
  chạy phía máy chủ sinh nonce (như tsudev.com). Web Analytics và Bot Fight Mode đã tắt (chủ dự án, 10/11).
- 10/11/2026 - Dependabot NuGet bỏ qua bản major (PR #27): bản major chờ upstream KnifeLemon/Filee nâng rồi gộp về.
  Từ 11/11/2026 (PR #36) bỏ qua thêm: PDFsharp >= 6.3 (AGPL, kiểm license lúc chạy), PDFtoImage >= 5.3, Svg.Skia
  >= 5.2, SkiaSharp >= 4 (xung đột Avalonia 12.1), Unhwp (ghim cùng `build/build-native.py`).
- 10/11/2026 - Giữ tên lệnh `filee` (không đổi thành `tsudev-files`): script ngoài phụ thuộc, ADR-0001. Muốn có tên
  phụ `tsudev-files` thì cần ADR riêng.

- 09/11/2026 - Spending limit GitHub Actions giữ 0 USD; Dependabot chạy hằng tháng và gom nhóm (PR #16) để tiết kiệm phút.

- 09/11/2026 - Tên hiển thị là `tsudev-files`, định danh nội bộ giữ `Filee` (ADR-0001, `docs/adr/0001-doi-ten-hien-thi-tsudev-files.md`); tệp phát hành đổi thành `tsudev-files-<ver>-...`, trình cập nhật nhận cả hai tiền tố.
- 09/11/2026 - Giữ tên namespace/assembly `Filee.*` để gộp upstream dễ; mọi khác biệt của fork gom vào `src/Filee.Core/ForkDefaults.cs` + `Directory.Build.props`.
- 09/11/2026 - Không sửa em-dash trong mã kế thừa từ upstream (48 file, gồm dữ liệu test cố ý chứa ký tự ngoài Latin-1): sửa sẽ gây xung đột khi gộp và làm yếu test. Mã MỚI của fork tuân thủ mục 7.1. Từ 10/11/2026 các dòng đó được miễn theo dòng qua `.standards-upstream` (quy ước v3.3.0); khi gộp upstream mới thì cập nhật `ref` trong file đó.
- 09/11/2026 - Khóa ký cập nhật ECDSA P-256 riêng; khóa bí mật ở `~/.config/tsudev-files/keys/update-key.pem` (0600, ngoài repo). Bản phát hành của upstream KHÔNG được bản fork chấp nhận.

- 09/11/2026 - ~~Giữ nút tài trợ trỏ về tác giả gốc KnifeLemon~~ - THAY bằng quyết định 11/11/2026 "Ủng hộ chỉ trỏ về tsudev".
- 09/11/2026 - Repo chuyển sang `tsudev-org` và thành Public (Actions miễn phí, CodeQL, Secret Scanning, runner Windows ARM).
  Hạng sản phẩm: C - Ngoại tuyến.

## Sự cố bảo mật

- (chưa có)
