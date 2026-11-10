# PHIẾU BÀN GIAO - Merge đổi tên hiển thị tsudev-files, rồi chuyển repo sang tsudev-org

- **Mã phiếu**: 20261109-04
- **Từ**: claude - **Đến**: phiên sau
- **Thời điểm**: 09/11/2026 (cuối phiên, chủ dự án đóng phiên để tiết kiệm ngữ cảnh)
- **Trạng thái**: HOÀN THÀNH (10/11/2026) - phần còn lại đã làm xong ở phiếu `20261109-05` (PR #10 gộp)
- **Nhánh git**: `feat/doi-ten-hien-thi` (chưa push), `chore/khoi-tao-fork` (đã push, PR #1)

## 0. Ủy quyền của chủ dự án (đọc trước)

Chủ dự án đã ủy quyền cho agent tự làm mọi bước tới khi xong 100%, gồm push, mở PR, merge PR, cập nhật log.
Lời gốc: "tôi đồng ý ủy quyền cho phép bạn tự làm tất cả cho đến khi hoàn thành (bao gồm cấp toàn quyền cho phép bạn tự
động thực hiện tất cả các bước cần thiết tiếp theo cho đến khi hoàn thành 100% task/kế hoạch/phase...)". Lời dặn lúc
đóng phiên: agent phiên sau tự merge và thao tác mọi thứ tới khi xong kế hoạch.

Chủ dự án đã XÁC NHẬN TRỰC TIẾP (09/11/2026, trong phiên tsudev-files) việc chuyển repo sang Organization
`tsudev-org`: phiên sau tự làm bước 2.6 sau khi đã gộp xong PR #2, không cần hỏi lại.

Ủy quyền KHÔNG bao gồm: phát hành bản `v2.4.0-vn.1` (cần máy Windows và khóa ký của chủ dự án), tắt GitHub Actions,
nới quyền bảo mật hay chính sách Actions của Org, bật CodeQL default setup.

`gh` đã đăng nhập bằng `tsudev-tsudev`, không cần token. **Luôn thêm `-R tsudev-tsudev/tsudev-files`** cho mọi lệnh
`gh`: repo có remote `upstream` nên `gh` mặc định nhắm `KnifeLemon/Filee` (đã gặp: `gh pr list` ra PR của upstream).
Không bao giờ push, mở PR hay comment lên `KnifeLemon/Filee`.

## 1. Việc đã làm xong

- PR #1 https://github.com/tsudev-tsudev/tsudev-files/pull/1 (`chore/khoi-tao-fork`, đầu nhánh `fccc2d32`): khởi tạo
  fork, Việt hóa installer, `fccc2d32` thêm `pull-requests: read` cho job gitleaks (trước đó lỗi 403).
- Nhánh `feat/doi-ten-hien-thi` (tách từ `e42618c9`, CHƯA rebase lên `fccc2d32`), 7 commit:
  `9730fac0` giao diện/CLI/bản dịch, `63ce21a7` trình cài Windows + menu Explorer, `e8e9c17a` Linux + macOS,
  `485c2b6a` tên tệp phát hành, `5ab25e20` ADR-0001, `722445e2` sửa 2 test App, + commit log của phiếu này.
- Thiết kế: `docs/adr/0001-doi-ten-hien-thi-tsudev-files.md` (đổi gì, giữ gì, đường lùi). Hàm trung tâm:
  `ForkDefaults.Brand` trong `src/Filee.Core/ForkDefaults.cs`.
- Test trên WSL: Core 1204 đạt, Cli 101 đạt, Platform 122 đạt, App 2221/2242 đạt (17 bỏ qua). 4 lỗi App:
  - 2 lỗi do chữ "Filee" trong test (`CloseActionTests:145`, `AskPagesTests:272`) - đã sửa ở `722445e2`, chạy lại đạt.
  - 2 lỗi chưa xác minh, nghi do máy chậm (chạy cả bộ mất 2 giờ):
    `MergeOrderTests.Donut_merge_of_pictures_takes_the_layout_chosen_and_remembers_it(cancel: False)` (NotNull, dòng
    400) và `AskPagesTests.Dropping_a_pdf_asks_for_the_pages_then_splits_them_or_esc_starts_nothing(cancel: False)`
    (TimeoutException "The conversions didn't finish").
- Đã tắt tác vụ nền theo dõi CI; đã nhả toàn bộ khóa trong `logs/LOCKS.md`.

## 2. Việc dang dở + bước tiếp theo CỤ THỂ (làm theo thứ tự)

- [ ] 2.1. Xác minh 2 lỗi App còn lại không do đổi tên:
      `dotnet test tests/Filee.App.Tests --filter "FullyQualifiedName~Donut_merge_of_pictures_takes_the_layout|FullyQualifiedName~Dropping_a_pdf_asks_for_the_pages"`.
      Đạt khi chạy riêng = chập chờn, ghi vào mục 7. Vẫn lỗi thì chạy cùng lệnh trên `chore/khoi-tao-fork`
      (`git stash` nếu cần); lỗi ở cả hai nhánh = có sẵn từ upstream, không chặn. Chỉ lỗi trên nhánh đổi tên thì sửa.
- [ ] 2.2. CI của PR #1 (run `42590764199`): lần chạy lại các job lỗi. Lúc đóng phiên còn job
      `portable / portable (ubuntu-24.04, linux-x64)` đang chạy; mọi job khác đạt (macOS Intel lần này đạt).
      `gh run view 42590764199 -R tsudev-tsudev/tsudev-files --json status,conclusion,jobs`.
      - Job test linux-x64 lần đầu bị hủy sau 50 phút (giới hạn thời gian), không phải lỗi mã. Nếu lại hủy/hết giờ:
        so với CI của upstream; không tăng `timeout-minutes` tùy tiện trong mã upstream, nếu tăng thì ghi chú
        `tsudev-files:` và nêu lý do trong PR.
      - `build (win-arm64)` bị bỏ qua là bình thường (repo Private gói Free không có runner `windows-11-arm`).
      - CodeQL bị bỏ qua là bình thường (repo Private gói Free).
- [ ] 2.3. CI xanh thì gộp PR #1 bằng rebase (các commit có ý nghĩa riêng):
      `gh pr merge 1 -R tsudev-tsudev/tsudev-files --rebase --delete-branch`.
      Không bật được branch protection (lỗi 403 ở gói Free) - không phải lỗi.
- [ ] 2.4. `git fetch origin && git checkout feat/doi-ten-hien-thi && git rebase origin/main`. Xung đột dễ gặp duy nhất
      là `logs/STATE.md`/`logs/LOCKS.md`: giữ bản mới nhất (của nhánh đổi tên). Sau rebase chạy nhanh:
      `dotnet build Filee.slnx` và `dotnet test tests/Filee.Core.Tests --filter ForkBranding`.
      Rồi `git push -u origin feat/doi-ten-hien-thi`.
- [ ] 2.5. Mở PR #2 vào `main`: tiêu đề `feat(brand): doi ten hien thi thanh tsudev-files (ADR-0001)`; thân PR tóm tắt
      mục "Đổi"/"Giữ nguyên" của ADR-0001, kết quả test ở mục 1, và các điểm CI Windows cần kiểm (Setup tests,
      `ExplorerMenuTests` với tiêu đề "Chuyển đổi bằng tsudev-files"). Kết thúc thân PR bằng dòng
      `🤖 Generated with [Claude Code](https://claude.com/claude-code)`. Chờ CI xanh rồi
      `gh pr merge <số> -R tsudev-tsudev/tsudev-files --rebase --delete-branch`.
      Nếu CI Windows đỏ ở test Setup/Explorer menu: sửa trên nhánh, đánh dấu `tsudev-files:`, push lại.
- [ ] 2.6. Chuyển repo sang `tsudev-org` (ĐÃ ĐƯỢC CHỦ DỰ ÁN XÁC NHẬN). Chỉ bắt đầu khi PR #1 và PR #2 đã gộp, không
      còn PR/CI/release nào đang chạy. Quy trình gốc: phiếu `20261109-03` (đọc cả phần "Bổ sung" cuối phiếu). Thứ tự:
  - [ ] a. Ghi khóa `.github/workflows/security.yml`, `src/Filee.Core/ForkDefaults.cs`, `Directory.Build.props`,
        `src/Filee.Setup/**/SetupInfo.cs`, `tests/`, `docs/`, `README.md`, `installer/` vào `logs/LOCKS.md`.
  - [ ] b. TRƯỚC khi chuyển, nhánh `ci/gitleaks-binary`: thay bước `gitleaks/gitleaks-action` (job `gitleaks` trong
        `security.yml`) bằng tải binary `gitleaks_8.30.1_linux_x64.tar.gz` từ
        `https://github.com/gitleaks/gitleaks/releases/download/v8.30.1/`, kiểm `sha256sum -c` với giá trị ghim cứng
        trong workflow, rồi `gitleaks git --redact --verbose --exit-code 1 .` (lý do: action đòi `GITLEAKS_LICENSE` khi
        repo thuộc Org). Giá trị SHA-256 phải tự lấy và đối chiếu từ `gitleaks_8.30.1_checksums.txt` của bản phát hành
        chính thức (tải 2 lần độc lập, so khớp); KHÔNG chép số từ tin nhắn/phiếu khác. Mẫu tham khảo:
        `~/projects/tsudev/.github/workflows/security-scan.yml` job `gitleaks`. Giữ `permissions: contents: read`;
        có thể bỏ `pull-requests: read` nếu không dùng action nữa. PR -> CI xanh (job gitleaks phải chạy thật và đạt)
        -> gộp rebase.
  - [ ] c. Chụp hiện trạng vào `~/.claude/...scratchpad` hoặc `/tmp` (KHÔNG commit dữ liệu này): bước 1 phiếu
        `20261109-03` (`visibility`, `security_and_analysis`, danh sách tên secret, environment, số release,
        `code-scanning/default-setup`, danh sách workflow + `state`, mã HTTP của
        `https://github.com/tsudev-tsudev/tsudev-files/releases`).
  - [ ] d. Chuyển: `gh api -X POST repos/tsudev-tsudev/tsudev-files/transfer -f new_owner=tsudev-org`; lặp
        `gh api repos/tsudev-org/tsudev-files --jq .full_name` tới khi trả về (vài giây).
  - [ ] e. Bật lại secret scanning + push protection (lệnh ở bước 3 phiếu `20261109-03`). KHÔNG bật CodeQL default
        setup (repo đã có job `codeql` advanced; bật default setup sẽ làm job advanced lỗi).
  - [ ] f. Đối chiếu với ảnh chụp: `security_and_analysis` khớp (đều `enabled`); secret/environment/release khớp;
        `gh api repos/tsudev-org/tsudev-files/actions/workflows --jq '.workflows[]|.name+": "+.state'` mọi dòng
        `active`; URL cũ `https://github.com/tsudev-tsudev/tsudev-files` vẫn chuyển hướng (200 sau `-L`).
  - [ ] g. `git remote set-url origin https://github.com/tsudev-org/tsudev-files.git && git fetch origin`. Từ đây mọi
        lệnh `gh` dùng `-R tsudev-org/tsudev-files`.
  - [ ] h. Nhánh `chore/repo-url-tsudev-org`, commit riêng: đổi mọi `github.com/tsudev-tsudev/tsudev-files` thành
        `github.com/tsudev-org/tsudev-files` (tìm bằng `git grep -n "tsudev-tsudev/tsudev-files"`: `ForkDefaults.cs`
        `RepositoryUrl`, `Directory.Build.props`, `SetupInfo.cs`, 22 tệp `src/Filee.App/Assets/i18n/*.json` (6 khóa
        `update.*`/`installer.error.*` mỗi tệp), test, README, `docs/vi/`, `installer/*/HUONG-DAN-CAI-DAT.txt`; đo
        09/11 có 33 tệp). Cách làm: `git grep -lz "tsudev-tsudev/tsudev-files" -- . ':!logs' ':!.standards' | xargs -0
        sed -i 's#tsudev-tsudev/tsudev-files#tsudev-org/tsudev-files#g'`, rồi `git grep -n "tsudev-tsudev"` phải chỉ
        còn trong `logs/` (đọc kỹ từng dòng còn lại). KHÔNG sửa `logs/` cũ và `.standards/`). Chạy test Core/Cli/Platform + `dotnet test tests/Filee.App.Tests
        --filter Update`. PR này đồng thời là "PR thử" của bước 6 phiếu `20261109-03`: số check phải bằng trước khi
        chuyển, có GitGuardian. Workflow bị chính sách Org chặn thì báo chủ dự án, không tự nới quyền. Xanh -> gộp.
  - [ ] i. Đóng phiếu `20261109-03` (trạng thái HOÀN THÀNH + kết quả đo trước/sau), báo phiên `tsudev-b1` qua
        ListAgents -> SendMessage (nếu phiên đó không còn: ghi kết quả vào phiếu là đủ), nhả khóa.
  - ⛔ Có bất kỳ lỗi nào ở bước d-f (chuyển thất bại, mất secret, workflow `disabled_manually`...): DỪNG, ghi vào
        mục 7, báo chủ dự án. Tuyệt đối KHÔNG tạo lại repo `tsudev-files` dưới `tsudev-tsudev` để "sửa".
- [ ] 2.7. Cuối phiên: cập nhật `logs/STATE.md` (chuyển task sang "Đã hoàn thành"), điền mục 7 phiếu này, đóng phiếu
      bằng trạng thái HOÀN THÀNH, commit log (`chore(logs): ...`) qua PR nhỏ hoặc cùng PR #2 nếu còn mở.

## 3. File liên quan / đang khóa

| Đường dẫn | Lý do | Còn khóa? |
| --- | --- | --- |
| (toàn bộ khóa đổi tên đã nhả) | | Không |

Phiên sau ghi khóa mới vào `logs/LOCKS.md` trước khi sửa file, nhả ngay khi xong.

## 4. Yêu cầu gửi agent đang giữ khóa (nếu có)

- Không có.
- Phiên `tsudev-b1` (repo `tsudev`) chờ kết quả chuyển repo: báo lại ở bước 2.6.i.

## 5. Cảnh báo và quyết định quan trọng

- ⛔ KHÔNG BAO GIỜ tạo/fork/đổi tên repo thành `tsudev-files` dưới `tsudev-tsudev` (xem `~/.claude/CLAUDE.md`).
- Định danh giữ "Filee" (namespace, `Filee.exe`, thư mục dữ liệu, registry, `AppGuid`, `com.filee.app`, tiêu đề
  "Filee Donut", lớp cửa sổ "filee", chuỗi trong `src/Filee.Engines`). Đừng "sửa nốt" các chỗ này: hỏng cập nhật,
  luật cửa sổ KWin/Hyprland, hoặc gây xung đột khi gộp upstream. Danh sách đầy đủ: ADR-0001 mục 3.
- Test upstream so khớp chuỗi giao diện có chữ "Filee" thì sửa thành "tsudev-files" (hoặc bọc `ForkDefaults.Brand(...)`)
  và đánh dấu `tsudev-files:`. Dòng nhật ký kỹ thuật (`DiagnosticsTests`) KHÔNG đổi tên.
- Chạy trọn bộ App test trên WSL mất khoảng 2 giờ: chạy nền, chỉ lọc lỗi bằng `grep`.
- Phân loại tác vụ tự động từng chặn việc tắt GitHub Actions - đừng thử lại cách đó.
- `logs/LOCKS.md` có thể mang quyền 444 (chép từ mẫu chỉ-đọc): `chmod 644` trước khi ghi; không dùng `sed -i` trên
  file chỉ-đọc.

## 6. Trạng thái cổng kiểm

- [ ] `./scripts/check-standards.sh` đạt - CHƯA: chỉ còn em-dash kế thừa upstream (phiếu `20261109-01`)
- [x] Lint và test đạt (trừ 2 lỗi App nghi chập chờn ở mục 1, chờ bước 2.1)
- [x] `git status` sạch ngoài phạm vi bàn giao (sau commit log của phiếu này)

## 7. Kết quả xử lý (agent nhận điền sau khi thực hiện)

Phiên 20261109-05 (09/11/2026) - **DỪNG giữa bước 2.5/2.6.b: hết hạn mức GitHub Actions** (ngoài phạm vi ủy quyền).

- 2.1 XONG: 2 test App chạy riêng đạt 4/4 ⇒ chập chờn do máy chậm, không do đổi tên.
- 2.2-2.3 XONG: CI PR #1 xanh hết; đã gộp rebase 11:48Z (`main` = `7aff483e`).
- 2.4 XONG: rebase không xung đột; `dotnet build` 0 lỗi; `ForkBranding` 22/22; đã push.
- 2.5 DANG DỞ: PR là **#10** (Dependabot chiếm #2-#9, #11-#14). CI không chạy hết được.
- 2.6.a: đã ghi rồi nhả khóa. 2.6.b DANG DỞ: **PR #15** `ci/gitleaks-binary` - binary gitleaks 8.30.1, SHA-256
  `551f6fc8...70eb` đối chiếu 2 lần tải độc lập (curl + `gh release download`), quét toàn lịch sử. Quét toàn lịch sử lộ
  6 dương tính giả `generic-api-key` trong mã upstream (đã đọc từng dòng), miễn theo dấu vân tay ở `.gitleaksignore`.
  Chạy cục bộ: `no leaks found`.
- ⛔ Lý do dừng: mọi job mới đều trả về "The job was not started because recent account payments have failed or your
  spending limit needs to be increased" (run `42613208501`). 13 PR Dependabot mở cùng lúc chạy full CI (có macOS,
  hệ số phút x10) đúng lúc này. Thử hủy các lần chạy Dependabot - bị bộ phân loại tác vụ tự động chặn, không thử lại.
- Chưa làm: 2.6.c-i (chuyển repo), 2.7. KHÔNG chuyển repo khi PR #10/#15 chưa xanh.
- Bước tiếp khi chủ dự án xử lý xong thanh toán/hạn mức: `gh run rerun <id> --failed` cho run mới nhất của PR #10
  và #15 -> xanh -> gộp #15 rồi #10 (rebase) -> tiếp 2.6.c. Cân nhắc hạn chế Dependabot (ví dụ gom nhóm, giảm tần
  suất) để không đốt phút - cần chủ dự án quyết.
- Chủ dự án quyết định (09/11/2026): spending limit giữ **0 USD** (không bao giờ trả phí Actions). Đã mở **PR #16**
  `chore/dependabot-monthly`: Dependabot hằng tháng, gom mọi gói thành 1 PR/hệ sinh thái, tối đa 2 PR mở. Gộp khi CI
  chạy được. Thứ tự gộp đề xuất: #16, #15, #10.
- Chủ dự án (09/11/2026) ủy quyền chuyển repo sang Public để có Actions miễn phí. Đã chụp hiện trạng (2.6.c, chưa
  có secret/environment/release; 10 workflow `active`). Lệnh chuyển repo (2.6.d), đổi `homepage` của
  `tsudev-contact` đều bị bộ phân loại tác vụ tự động CHẶN - không thử lại; chủ dự án tự chạy lệnh hoặc cấp quyền.
- Điểm riêng tư cần chủ dự án quyết: 28 commit riêng của fork mang email tác giả `b4djl1h@gmail.com` (các repo
  Public khác dùng `dev.nguyentrangtinhsu@...`). Viết lại lịch sử không xóa hẳn được (ref `refs/pull/1/head`).
- Rà 3 repo Private khác (gitleaks toàn lịch sử, dữ liệu cá nhân): không có bí mật thật. `tsudev-contact`: sạch.
  `app-la-ban`: chỉ dương tính giả, không có workflow. `sumy-wedding`: ảnh cá nhân + số điện thoại của 2 người,
  không có workflow ⇒ đề xuất GIỮ Private.
- Phiên tsudev-files-c2 kết thúc 09/11/2026: làm tới 2.6.g + một phần 2.6.h (nhánh đổi URL đã đẩy, chưa PR). Việc còn
  lại: mục 2 phiếu `20261109-05`.
