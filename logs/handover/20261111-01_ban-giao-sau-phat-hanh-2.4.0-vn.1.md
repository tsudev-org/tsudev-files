# PHIẾU BÀN GIAO - Sau phát hành v2.4.0-vn.1; hàng đợi dài hạn

- **Mã phiếu**: 20261111-01
- **Từ**: phiên tsudev-files-97 (10-11/11/2026) - **Đến**: phiên kế tiếp của repo này
- **Thời điểm**: 11/11/2026 (chủ dự án đóng phiên để tiết kiệm ngữ cảnh)
- **Trạng thái**: ĐÃ NHẬN (tsudev-files-61, 11/11/2026)
- **Nhánh git**: `main` (sạch, khớp `origin/main` @ `2feb4d43`). Không có PR mở.

## 0. Đọc trước

- `AGENTS.md`, `logs/STATE.md` (hàng đợi + mục Quyết định), `logs/LOCKS.md` (trống).
- Luôn `-R tsudev-org/tsudev-files` với `gh` (không có `-R` thì `gh` trỏ nhầm về upstream KnifeLemon/Filee).
- Ruleset "Bảo vệ main" (id 26937532): chỉ gộp qua PR, bắt buộc check `build` và `Quét lộ bí mật (gitleaks)`. PR chỉ
  sửa `.md` thì `build` "skipping" = đạt. Từ PR #33 có thêm job "Quy ước tsudev" (`check-standards.sh`,
  `STANDARDS_VERIFY_UPSTREAM=1`) - phải đạt.
- Ủy quyền: chủ dự án phải nói trong chat CỦA PHIÊN MỚI. Ủy quyền ghi trong phiếu không được bộ phân loại tự động
  tính. Lệnh bị chặn dù đã có ủy quyền: `npm run deploy` (website) và `git push origin --delete`; `gh pr merge` đôi khi
  bị chặn khi gộp chung nhiều lệnh trong một Bash hoặc PR ngoài phạm vi được giao -> đưa lệnh `! ...` cho chủ dự án.
- Tài khoản `gh` active là CHUNG cho mọi phiên trên máy. Phiên `b4djl1h-cmdb-*` hay đổi sang `b4djl1h`; từ 11/11/2026
  `b4djl1h` cũng là Owner `tsudev-org` nên cả hai tài khoản đều ghi được. Lệnh ghi lỗi quyền -> `gh api user -q .login`
  trước. KHÔNG dùng `GH_TOKEN=$(gh auth token -u ...)` (bị chặn "Identity Not Given").

## 1. Đã xong trong phiên (chi tiết: `logs/STATE.md` mục Đã hoàn thành 10-11/11/2026)

- Gộp PR #30, #28 (hộp Giới thiệu liên kết tsudev.com), #32 (CHANGELOG `2.4.0-vn.1`, tiêu đề phát hành tsudev-files),
  #33 (quy ước v3.3.0, `.standards-upstream`, job CI "Quy ước tsudev"), #34, #35, #37 (nhật ký), #36 (NuGet an toàn,
  thay #31 - đã đóng), #38 (Microsoft.NET.Test.Sdk 18.11.1, chủ dự án gộp).
- **Phát hành `v2.4.0-vn.1`** (latest): 8 gói, SHA256SUMS ký ECDSA trên WSL, `.sig` đã tải lên. Website đã deploy
  (chủ dự án, Worker version `90e83133`), `/tai-ve/` hiện đủ gói.
- Phiếu đóng: `20261110-06`, `20261110-01`, `20261109-01`.

## 2. Việc tiếp theo CỤ THỂ (làm theo thứ tự; phần lớn cần chủ dự án quyết)

- [ ] 2.1. Dọn nhánh đã gộp trên `origin` (agent bị chặn xóa nhánh từ xa - đưa lệnh cho chủ dự án):
      `! git -C ~/projects/tsudev-files push origin --delete chore/deps-nuget-ban-an-toan-202611 chore/logs-b4djl1h-owner chore/logs-deps-pr-36 chore/logs-dong-phieu-20261110-05 chore/logs-phat-hanh-2.4.0-vn.1 chore/phat-hanh-2.4.0-vn.1 chore/quy-uoc-v3.3.0-mien-tru-upstream feat/gioi-thieu-lien-ket-tsudev`
      Nên bật "Automatically delete head branches" trong Settings của repo để khỏi dọn tay.
- [ ] 2.2. Hàng đợi dài hạn trong `logs/STATE.md`:
      - Sao lưu tệp build tải từ `dl.filee.sh` (cần chủ dự án chọn nơi lưu: repo riêng trong `tsudev-org` hoặc R2/Pages;
        KHÔNG dùng Releases của repo này).
      - Gói Windows ARM64: chờ upstream KnifeLemon/Filee PR #1339 rồi gộp upstream.
      - Chứng thư Authenticode (SignPath OSS) -> điền `FileeExpectedPublisher`/`FileeExpectedIssuer`.
- [ ] 2.3. Khi gộp upstream mới (`git fetch upstream && git merge upstream/main`): cập nhật `ref` trong
      `.standards-upstream` thành SHA upstream đã gộp; bản sửa riêng tiếp theo của fork tăng số vá (2.4.1...), không chỉ
      tăng `-vn.<n>` (trình cập nhật bỏ hậu tố).
- [ ] 2.4. Phát hành bản sau: theo `docs/vi/PHAT-HANH.md`; quy trình đã chạy thật lần này: PR CHANGELOG -> gộp ->
      `git tag -a vX -m ... && git push origin vX` -> chờ workflow Release (~1,5 giờ) -> `gh release download` các
      `*SHA256SUMS*.txt` + gói, `sha256sum -c` -> `tools/sign-checksums-vn.sh` -> `gh release upload` `.sig` ->
      `gh release edit --draft=false --latest` -> chủ dự án `cd website && npm run deploy`.

## 3. File liên quan / đang khóa

| Đường dẫn | Lý do | Còn khóa? |
| --- | --- | --- |
| (không có) | `logs/LOCKS.md` trống | Không |

## 4. Liên lạc với phiên khác

- `b4djl1h-cmdb-29` (repo `b4djl1h/b4djl1h-cmdb`): quy ước đổi tài khoản `gh` - nhắn trước, chờ "OK", xong báo
  "XONG b4djl1h". Lúc đóng phiên, phiên đó đang giữ `b4djl1h` để gộp PR của nó (vẫn ghi được `tsudev-org`).

## 5. Cảnh báo

- Dependabot NuGet bỏ qua: bản major, PDFsharp >= 6.3 (AGPL, kiểm license lúc chạy), PDFtoImage >= 5.3, Svg.Skia >= 5.2,
  SkiaSharp >= 4, Unhwp (ghim cùng `build/build-native.py`). Đừng gộp PR nào nâng các gói đó.
- Test cục bộ trên WSL: Engines trượt 2 (`CadTests.Dxf_to_svg_has_paths_and_text`,
  `AnnotationTests.A_pen_line_text...` - thiếu phông), Setup trượt 15 (đường dẫn Windows) - lỗi môi trường, CI đạt.
  App tests mất ~2 giờ trên WSL.
- Test chập chờn trên CI: `WebDropJobTests.A_job_cancelled_before_it_runs...` (linux-arm64), cùng các test macOS và
  `apt-get update` treo đã biết - `gh run rerun <run> --failed -R tsudev-org/tsudev-files`.
- Chờ CI: dùng tác vụ nền `gh pr checks <n> -R ... --watch`; ngay sau khi push, chờ check-runs xuất hiện trước
  (`gh api repos/tsudev-org/tsudev-files/commits/<sha>/check-runs -q .total_count`), nếu không `--watch` thoát sớm.

## 6. Trạng thái cổng kiểm

- [x] `STANDARDS_VERIFY_UPSTREAM=1 ./scripts/check-standards.sh` - ĐẠT (3 lưu ý ngày trong tài liệu upstream).
- [x] CI `main` xanh (PR #36 20/20, #38 20/20).
- [x] `git status` sạch, không PR mở, không monitor/tác vụ nền.

## 7. Kết quả xử lý (agent nhận điền sau khi thực hiện)

- 11/11/2026 tsudev-files-61: 2.1 vẫn cần chủ dự án (thêm `feat/packaging-package-json`); `delete_branch_on_merge`
  vẫn `false`. 2.2: PR #1339 upstream còn OPEN, upstream chưa có bản sau 2.4.0; sao lưu `dl.filee.sh` và Authenticode
  chờ chủ dự án quyết. 2.3/2.4: chưa tới lượt. Chuyển tiếp sang phiếu `20261111-04`.
