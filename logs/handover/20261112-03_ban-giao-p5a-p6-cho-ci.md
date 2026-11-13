# PHIẾU BÀN GIAO - P5a (#46) và P6 (#47) chờ sửa thứ tự using rồi gộp; Ủng hộ đồng bộ hệ sinh thái

- **Mã phiếu**: 20261112-03
- **Từ**: phiên tsudev-files-01 (11-12/11/2026) - **Đến**: phiên kế tiếp của repo này
- **Trạng thái**: CHỜ NHẬN
- **Nhánh git**: làm việc trên `feat/p6-thuong-hieu-va-ung-ho` @ `7f6103b3` (xếp trên `feat/p5a-doi-ten-ma-nguon` @
  `93524a49`, trên `main` @ `70768d88`). Nhật ký (`logs/STATE.md`, phiếu `20261111-05`, `20261112-02`, phiếu này) CHƯA
  commit - đưa vào PR nhật ký riêng từ `main` (mục 2.4).

## 0. Đọc trước

- Phiếu `20261111-01` mục 0 (gh `-R`, ruleset, lệnh bị chặn) vẫn đúng. `gh` active `tsudev-tsudev`; trước lệnh ghi chạy
  `gh api user -q .login`. Agent bị chặn deploy và xóa nhánh từ xa: đưa lệnh `!` cho chủ dự án.
- Ảnh `1794415871165_..._c60907999d26b8465e8a3ae3a6eaf32b.jpg` ở gốc repo là ảnh QR VIB chủ dự án gửi: **KHÔNG commit**
  (đã sinh SVG thay); để chủ dự án tự xóa/cất.
- Test cục bộ trên WSL: bộ App mất hơn 3 giờ khi máy tải nặng - đừng chạy cả bộ, dùng `--filter` và để CI chạy đủ.
  Setup trượt khoảng 65 trên WSL (registry, đường dẫn `C:\`, sandbox không ghi `/tmp`) - môi trường, không phải lỗi.

## 1. Đã xong trong phiên

- Gộp PR #45 (P4 fuzz, `4e2dfd8a`) và #43 (P2 SBOM, `70768d88`). Lỗi CI trước đó: DNS `azure.archive.ubuntu.com` của
  runner (14:03-16:34 UTC 11/11) và một test thời gian chập chờn - không phải mã.
- P5a dựng lại trên `main` mới bằng `tools/doi-ten-tsudev.py` (gồm cả `ForkFuzzTests` của P4) -> PR #46. Công cụ: bỏ qua
  `docs/adr/`, `--tep` nhận `.patch`/`.diff`.
- P6 -> PR #47 (2 commit trên P5a): logo riêng + dây chuyền `tools/thuong-hieu/tao-tai-san.py`, `AppLogo` thay
  `DonutLogo`; trang `files.tsudev.com/ung-ho/` kênh VIB `056485506` (NGUYEN TRANG TINH SU), QR SVG sinh từ chuỗi
  VietQR đối chiếu từng byte với mã VIB (`tools/ung-ho/tao-vietqr.py`), build kiểm CRC/BIN/số tài khoản + ghim SHA-256.
  Cục bộ: 255/255 test App liên quan, website build + a11y 0 lỗi, `check-standards.sh` ĐẠT.
- Phiếu hệ sinh thái: gốc `~/projects/logs/handover/20261112-01_yeu-cau-dong-bo-ung-ho-he-sinh-thai.md`, lựa chọn
  theo nền tảng `~/projects/logs/handover/20261112-02_quyet-dinh-ung-ho-theo-nen-tang.md` (Play: nút mở trang được;
  iOS: ẩn), phiếu con ở tsudev, lotus, trafficmonitor, cwico, swico, contact (lotus và trafficmonitor đã xử lý).
  Đề xuất quy ước: `logs/handover/20261111-05` (ngoại lệ logo), `20261112-02` (quy ước Ủng hộ).
- Chủ dự án đã đặt Social preview GitHub = `website/public/og-image.png`.

## 2. Việc tiếp theo (làm theo thứ tự)

- [ ] 2.1. **Sửa CI #46**: job `build` đỏ ở bước `dotnet format tsudev-files.slnx --verify-no-changes --no-restore
      --severity warn` - 214 lỗi `IMPORTS: Fix imports ordering` (đổi `Filee.*` -> `Tsudev.Files.*` làm lệch thứ tự
      `using`); CodeQL đỏ theo. Cách sửa trên nhánh `feat/p5a-doi-ten-ma-nguon`:
      `dotnet restore tsudev-files.slnx && dotnet format tsudev-files.slnx --no-restore --severity warn` rồi
      `dotnet format ... --verify-no-changes` phải sạch; build lại; commit `style: sap lai thu tu using sau doi ten`;
      push. Rồi `git rebase --onto feat/p5a-doi-ten-ma-nguon 93524a49 feat/p6-thuong-hieu-va-ung-ho`, chạy cùng lệnh
      format (P6 thêm `AppLogo.cs`, `SponsorLinks.cs`, test mới), `git push --force-with-lease`.
      Cân nhắc thêm bước `dotnet format` vào cuối `tools/doi-ten-tsudev.py` (hoặc ghi vào AGENTS.md B.4) để bản vá
      upstream chép về không lặp lỗi này.
- [ ] 2.2. Khi #46 xanh (bắt buộc: `build`, gitleaks): gộp `--merge`. Rồi #47 (tự còn 2 commit P6) xanh thì gộp.
      Job đóng gói Linux đỏ do `apt`/DNS thì chạy lại, không sửa mã.
- [ ] 2.3. Sau khi gộp #47: đưa chủ dự án `! cd website && npm run deploy`; nghiệm thu production `/ung-ho/` (giải mã
      QR trên trang ra đúng chuỗi VietQR trong `website/src/data/ung-ho.json`), favicon, og-image, đầu trang.
- [ ] 2.4. PR nhật ký `chore/logs-ban-giao-20261112-03` ĐÃ MỞ (gộp khi `build` xanh; nếu xung đột thì rebase). Nội dung: `logs/STATE.md` (đã cập nhật kế hoạch P1-P6, quyết định logo/Ủng hộ/nền tảng), phiếu
      `20261111-05`, `20261112-02`, phiếu này. Đánh dấu P2/P4 đã gộp, #46/#47 theo kết quả 2.2.
- [ ] 2.5. Rebase PR #42 (P1) và #44 (P3) lên `main` sau P5a: `git rebase origin/main`, chạy
      `python3 -I tools/doi-ten-tsudev.py` (đổi đường dẫn/định danh còn tên cũ), `dotnet format`, push. #42 vẫn chờ
      chủ dự án tạo repo `tsudev-org/tsufiles-assets`; #44 chờ khóa dự phòng.
- [ ] 2.6. P5b (bản 2.4.1, ADR-0002 mục 3): định danh lúc chạy + di trú; gom luôn tài nguyên upstream còn lại (STATE:
      GNOME `filee-donut@filee.sh`, `build/sign-release.ps1` + CONTRIBUTING gương R2, Help CLI "KnifeLemon/Filee").
- [x] 2.7. Chủ dự án ĐÃ DUYỆT (12/11/2026) đổi chữ gói Pro của `app-la-ban` - ghi ở
      `~/projects/logs/handover/20261112-02_quyet-dinh-ung-ho-theo-nen-tang.md` mục 3 cho phiên app-la-ban.
- [ ] 2.8. "Require review from Code Owners": chủ dự án tự bật trong ruleset "Bảo vệ main" **SAU khi gộp #47** (trên
      `main` CODEOWNERS còn `* @KnifeLemon` tới lúc đó). Khi đã bật: PR do `tsudev-tsudev` mở không tự duyệt được, nên
      gộp bằng `gh pr merge <n> -R tsudev-org/tsufiles --merge --admin` (vai trò Admin được bỏ qua khi gộp PR -
      `bypass_actors` RepositoryRole 5, mode `pull_request`); PR của Dependabot/người khác cần chủ dự án Approve.
      Kiểm: `gh api repos/tsudev-org/tsufiles/rulesets/26937532 -q '.rules[]|select(.type=="pull_request").parameters.require_code_owner_review'` = `true`.

## 3. File đang khóa

- (không có) - `logs/LOCKS.md` trống. Không còn monitor/tác vụ nền; worktree tạm đã gỡ.

## 4. Cổng kiểm

- [x] `--kiem` sạch, `check-standards.sh` ĐẠT trên cả P5a và P6. [ ] CI #46/#47 - đỏ vì thứ tự `using` (mục 2.1).

## 5. Kết quả xử lý (agent nhận điền)

-
