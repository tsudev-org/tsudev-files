# PHIẾU BÀN GIAO - Đã chuyển repo sang tsudev-org + Public; còn gộp PR #10, PR đổi URL, đóng phiếu

- **Mã phiếu**: 20261109-05
- **Từ**: phiên tsudev-files-c2 (09/11/2026) - **Đến**: phiên kế tiếp
- **Thời điểm**: 09/11/2026 tối (chủ dự án đóng phiên để tiết kiệm ngữ cảnh)
- **Trạng thái**: HOÀN THÀNH (10/11/2026), trừ 2.5/2.6 chờ chủ dự án quyết (chuyển vào `logs/STATE.md`)
- **Thay phiếu**: mục 2 phiếu `20261109-04` (đọc mục 7 của phiếu đó để biết lịch sử). Phiếu `20261109-03` còn MỞ.
- **Nhánh git**: `feat/doi-ten-hien-thi` (PR #10, đầu nhánh `c2ff4048`), `chore/repo-url-tsudev-org` (CHỈ đẩy nhánh,
  CHƯA mở PR; chứa cả commit nhật ký của phiếu này)

## 0. Ủy quyền (đọc trước)

Chủ dự án ủy quyền làm mọi bước tới khi xong 100%: push, mở PR, gộp PR, cập nhật log, chi phí 0 đồng. Đã XÁC NHẬN cụ
thể (09/11/2026): chuyển `tsudev-files` sang `tsudev-org`, đổi Public `tsudev-files`, `tsudev-contact`, `app-la-ban`
(GIỮ `sumy-wedding` Private: ảnh + số điện thoại cá nhân), chấp nhận lộ email tác giả `b4djl1h@gmail.com`, spending
limit Actions giữ **0 USD**, chuẩn hóa nhận diện *.tsudev.com.

KHÔNG gồm: phát hành `v2.4.0-vn.1` (cần Windows + khóa ký của chủ dự án), tắt Actions, nới quyền/chính sách Org, bật
CodeQL default setup ở repo NÀY (đã có job `codeql` advanced), đổi Public `sumy-wedding`.

**Luôn** `gh ... -R tsudev-org/tsudev-files` (remote `upstream` = KnifeLemon/Filee; `gh` mặc định nhắm upstream).

Lệnh từng bị bộ phân loại tác vụ tự động CHẶN (không lách; cần thì nhờ chủ dự án chạy `! <lệnh>`): `gh run cancel`
hàng loạt cho Dependabot, `git push --force-with-lease`. Thay force-push: `gh pr update-branch <n> -R ... --rebase`
(GitHub tự rebase phía máy chủ - đã dùng được cho PR #10), hoặc đẩy nhánh mới + mở PR mới.

## 1. Đã xong trong phiên

- PR #1 (khởi tạo fork) gộp. PR #16 (Dependabot hằng tháng, gom nhóm, tối đa 2 PR) gộp. PR #17 (gitleaks binary
  8.30.1 ghim SHA-256 `551f6fc8...70eb`, đối chiếu 2 lần tải; quét toàn lịch sử; `.gitleaksignore` miễn 6 dương tính
  giả upstream theo dấu vân tay) gộp. PR #15 đóng (thay bằng #17).
- 2.1: 2 test App nghi chập chờn chạy riêng đạt 4/4 -> chập chờn.
- Repo đã CHUYỂN sang `tsudev-org/tsudev-files` và **Public**. Bật lại: secret scanning, push protection, Dependabot
  alerts, private vulnerability reporting. CodeQL default setup: `not-configured` (đúng ý). `git remote origin` đã trỏ
  `tsudev-org`. Đối chiếu trước/sau (bước 1/4 phiếu 03):
  - Trước (tsudev-tsudev, Private): secret 0, environment 0, release 0, 10 workflow `active`, 15 PR mở,
    `security_and_analysis` null (Private Free), branch protection 403 (Private Free), default-setup 403.
  - Sau (tsudev-org, Public): secret 0, environment 0, release 0, 10/10 workflow `active`, 15 PR mở, secret scanning +
    push protection `enabled`, Actions `enabled/all`. URL cũ `github.com/tsudev-tsudev/tsudev-files` (+ `/releases`,
    `.git/info/refs`) trả 200 qua chuyển hướng.
  - PR thử (bước 6 phiếu 03): CI chạy đủ trên runner miễn phí, có GitGuardian, CodeQL (C#) và `build (win-arm64)`
    giờ CHẠY (trước bị bỏ qua vì Private). Chính sách Org không chặn workflow nào.
- `tsudev-org/tsudev-contact`, `tsudev-tsudev/app-la-ban`: đã Public + secret scanning + push protection + Dependabot
  alerts + PVR + CodeQL default setup (2 repo này KHÔNG có workflow CodeQL riêng).
- Nhận diện (ECOSYSTEM_IDENTITY 2.1) trên GitHub cho 8 repo: mô tả kết thúc ` | tsudev.com`, nhãn `tsudev`, homepage
  (`app-la-ban` -> `https://compass.tsudev.com`; repo chưa có trang -> `https://tsudev.com`).
- Phiếu yêu cầu đã viết (CHƯA commit, nằm ở repo đó): `tsudev/logs/handover/20261109-06_yeu-cau-trang-san-pham-tsudev-com.md`
  (website), `tsudev-{contact,swico,cwico}/logs/handover/20261109-01_yeu-cau-readme-nhan-dien-tsudev.md` (README),
  `app-la-ban`: `./agents.sh handoff` mã `HGLB-1109-212627` (dòng mới trong `AGENTS_LOG.md`, chưa commit). Phiên của
  các repo đó tự nhận và commit - repo này KHÔNG sửa hộ.
- Quyết định: giữ nút tài trợ trỏ về KnifeLemon; `dl.filee.sh` thành task riêng trong `logs/STATE.md`.

## 2. Việc tiếp theo CỤ THỂ (làm theo thứ tự)

- [x] 2.1. PR #10 `feat/doi-ten-hien-thi` (đã rebase phía máy chủ lên `main` có gitleaks binary; commit `c2ff4048`
      sửa 4 test `Filee.Setup.Tests.InstallerTests` mong tên cũ). `gh pr checks 10 -R tsudev-org/tsudev-files`.
      - Xanh -> `gh pr merge 10 -R tsudev-org/tsudev-files --rebase --delete-branch`.
      - Job `build` (Windows) đỏ ở `Filee.Setup.Tests`: đọc log (`gh api repos/tsudev-org/tsudev-files/actions/jobs/<id>/logs`),
        sửa test theo `SetupNames.ShortcutName/DisplayName/Publisher` (ADR-0001), đánh dấu `tsudev-files:`. Test Setup
        chỉ chạy được trên Windows (trên WSL lỗi 65 test kể cả khi chưa sửa - KHÔNG dùng để kết luận).
      - Job hủy vì thiếu runner/hết giờ: `gh run rerun <id> --failed -R tsudev-org/tsudev-files`.
      - Trước đó: `git fetch origin && git checkout feat/doi-ten-hien-thi && git reset --hard origin/feat/doi-ten-hien-thi`
        nếu nhánh local lệch (nhánh đã bị rebase phía máy chủ).
- [x] 2.2. Nhánh `chore/repo-url-tsudev-org` (đã đẩy, chưa PR), 4 commit trên nền nhánh đổi tên CŨ (`c0af7e34`):
      `chore(repo): doi URL repo sang github.com/tsudev-org/tsudev-files` (33 tệp), `chore(standards): nang len quy
      uoc v3.2.0 (tsudev-org)` (sync-standards.sh, đã xác minh MANIFEST), `docs(readme): dong gioi thieu tsudev va hang
      san pham C`, `chore(logs): ...` (phiếu này + STATE). Sau khi PR #10 gộp:
      `git fetch origin && git checkout chore/repo-url-tsudev-org && git rebase --onto origin/main c0af7e34`
      (xung đột dễ gặp: `logs/STATE.md`, phiếu 04 - giữ bản của nhánh này). Kiểm:
      `git grep -n "tsudev-tsudev" -- . ':!logs' ':!.standards'` chỉ còn `AGENTS.md` (người liên hệ), ADR-0001
      (người quyết định) - cả hai là TÊN NGƯỜI, giữ. Chạy test Core/Cli/Platform + `dotnet test tests/Filee.App.Tests
      --filter Update` + `python3 -I tools/check_i18n_vi.py`; `./scripts/check-standards.sh` phải còn đúng 49 vi phạm
      (em-dash upstream, phiếu `20261109-01`) - không tăng. Test Cli `Zip_to_iso...` lỗi nếu chạy trong worktree thiếu
      `engines/` (gitignore) - chạy trong thư mục repo chính. Đẩy bằng PUSH THƯỜNG (`git push origin HEAD`; nếu bị từ
      chối vì lịch sử khác: `git push origin HEAD:chore/repo-url-tsudev-org-v2` rồi mở PR từ nhánh v2, xóa nhánh cũ).
      Mở PR `chore(repo): doi URL sang tsudev-org, quy uoc v3.2.0, nhan dien README`, thân kết thúc bằng
      `🤖 Generated with [Claude Code](https://claude.com/claude-code)`. Xanh -> gộp `--rebase --delete-branch`.
- [x] 2.3. Đóng phiếu `20261109-03`: trạng thái HOÀN THÀNH + chép số đo trước/sau ở mục 1 phiếu này. Báo phiên
      `tsudev-b1` qua ListAgents -> SendMessage (09/11 phiên đó KHÔNG còn - ghi vào phiếu là đủ).
- [x] 2.4. Đóng phiếu 04 và phiếu này (HOÀN THÀNH, điền mục 7), cập nhật `logs/STATE.md` (chuyển task sang Đã hoàn
      thành, bỏ dòng ⛔ CHẶN hạn mức Actions vì repo đã Public), commit `chore(logs): ...` qua PR nhỏ -> gộp.
- [ ] 2.5. (Tùy chọn, cần chủ dự án duyệt) Repo Public nên giờ bật được ruleset bảo vệ `main` (trước 403). Đề xuất:
      chặn force-push/xóa `main`, bắt buộc PR + check `build`, `Quét lộ bí mật (gitleaks)`. Hỏi trước khi bật.
- [ ] 2.6. 13 PR Dependabot lẻ (#2-#9, #11-#14) sẽ được thay bằng PR nhóm ở lần chạy ngày 1 hằng tháng (cấu hình
      PR #16). Không gộp lẻ; nếu muốn dọn sớm: `gh pr close <n> -R tsudev-org/tsudev-files` (hỏi chủ dự án).

## 3. File liên quan / đang khóa

| Đường dẫn | Lý do | Còn khóa? |
| --- | --- | --- |
| (không có khóa nào của phiên này) | | Không |

Phiên sau ghi khóa vào `logs/LOCKS.md` trước khi sửa (`chmod 644` nếu file mang quyền 444).

## 4. Yêu cầu gửi agent khác

- Các phiếu ở mục 1 (tsudev, tsudev-contact, tsudev-swico, tsudev-cwico, app-la-ban). `tsudev-cwico` lúc viết phiếu có
  `ui/package-lock.json` sửa dở - không đụng.

## 5. Cảnh báo

- ⛔ KHÔNG BAO GIỜ tạo/fork/đổi tên repo thành `tsudev-files` (hay tên cũ nào) dưới `tsudev-tsudev`.
- Định danh "Filee" giữ nguyên (ADR-0001 mục 3) - đừng "sửa nốt".
- `.gitleaksignore`: KHÔNG trích nguyên văn đoạn mã bị bắt vào chú thích (chính chú thích bị bắt - đã gặp ở PR #15).
- Job macOS Intel là job chậm nhất (> 1 giờ); cả bộ CI 1-1,5 giờ. Chờ bằng tác vụ nền, đừng chạy vòng chờ tiền cảnh.
- Repo Private của tài khoản gói Free chỉ có 2.000 phút Actions/tháng (đã cạn 09/11); repo này nay Public nên không tốn.

## 6. Trạng thái cổng kiểm

- [ ] `./scripts/check-standards.sh` - 49 vi phạm, tất cả em-dash kế thừa upstream (chờ miễn trừ, phiếu `20261109-01`)
- [x] CI PR #10, #20, #21 - xanh và đã gộp (10/11/2026)
- [x] `git status` sạch trên `feat/doi-ten-hien-thi` (nhật ký đã chuyển vào nhánh `chore/repo-url-tsudev-org`)

## 7. Kết quả xử lý (agent nhận điền sau khi thực hiện)

Phiên tsudev-files 10/11/2026 (chủ dự án yêu cầu làm mục 2, kèm xây dựng website `files.tsudev.com`).

- 2.1 XONG: PR #10 gộp (rebase) sau 18/18 check xanh. Thêm vào PR này 4 sửa lỗi tìm được khi CI chạy đủ:
  - `fix(app)`: chữ "Filee" viết cứng ở thanh bên (`MainWindow.axaml`), trang Giới thiệu, màn chào Setup đổi sang
    `ForkDefaults.DisplayName`; test `ForkBrandingTests.No_view_shows_the_upstream_name_as_text`.
  - `fix(build)`: `build/build-installer.ps1` kiểm lối tắt `tsudev-files.lnk` thay vì `Filee.lnk` - nguyên nhân thật
    của job `build` Windows đỏ ("Setup did not create the desktop shortcut").
  - `fix(core)`: `ForkDefaults.Brand` bỏ regex có timeout 100 ms (gây `RegexMatchTimeoutException`, `filee help
    engines` sập 1/6 lần dưới tải) - thay bằng bộ quét ký tự O(n), đối chiếu với regex cũ trên 50.000 chuỗi ngẫu nhiên
    và mọi chuỗi dịch; đột biến 3 quy tắc đều bị bắt. Lỗi gốc không tái hiện được cục bộ (0/240 cả bản cũ), nên sửa
    dựa trên loại bỏ cơ chế gây lỗi. Lệnh `filee` GIỮ tên (ADR-0001, script ngoài phụ thuộc).
  - `style(test)`: `dotnet format`. Bài học: chạy `dotnet format Filee.slnx --verify-no-changes --no-restore
    --severity warn` TRƯỚC khi đẩy.
- 2.2 XONG: nhánh đổi URL rebase `--onto origin/main c0af7e34` sạch; push thường bị từ chối nên đẩy sang
  `chore/repo-url-tsudev-org-v2` -> **PR #21**, gộp 07:50Z. Cục bộ: build, format 0, Core 1227, Cli 101, Platform 122,
  App (Update) 107 đạt, i18n 1600/1600, `check-standards` 48 vi phạm (giảm từ 49, không tăng). Job macOS đỏ 2 lần ở
  test chập chờn khác nhau (`DonutFlipTests` Expected 2/Actual 1; `MemoryTrimmerTests` đo bộ nhớ) - chạy lại xanh.
- Website `files.tsudev.com` (thêm theo yêu cầu chủ dự án): **PR #20** gộp 07:50Z. Astro 7.4.7, trang tĩnh,
  nội dung lấy từ `.md` của repo; nhận diện theo ECOSYSTEM_IDENTITY 2.2/4; ảnh chụp có tên tsudev-files; host bằng
  Cloudflare Workers Static Assets (`website/wrangler.jsonc`, tên miền riêng tự gán - mẹo của phiên tsudev-swico:
  OAuth của wrangler không ghi được DNS nên Pages + CNAME không dùng được). Chủ dự án tự chạy `npm run deploy`
  (lệnh deploy bị bộ phân loại chặn với agent). Đo production: 9 trang 200, 404 đúng, http -> https 301,
  HSTS/CSP/X-Frame-Options đúng. Homepage repo -> `https://files.tsudev.com`. CI: workflow `website.yml`
  (build, check-dist, axe 0 lỗi); `ci.yml` bỏ qua thay đổi chỉ ở `website/`.
- 2.3, 2.4 XONG: phiếu 03, 04 và phiếu này đóng; `logs/STATE.md` cập nhật (PR nhật ký này).
- 2.5, 2.6 CHƯA LÀM - cần chủ dự án quyết, đã chuyển vào hàng đợi `logs/STATE.md`.
- Xóa nhánh remote đã gộp (`feat/website`, `chore/repo-url-tsudev-org`, `chore/repo-url-tsudev-org-v2`, mọi commit
  đã có trong `main` theo `git cherry`): lệnh bị bộ phân loại chặn ("Git Destructive") - chủ dự án tự xóa.
- Phiếu mới nhận trong phiên (commit kèm PR này): `20261110-01` (áp dụng miễn trừ em-dash v3.3.0 - chờ
  `tsudev-standards` PR #48), `20261110-02` (nút đổi giao diện - **PR #25**, chờ gộp + deploy + nghiệm thu).
