# PHIẾU BÀN GIAO - Đổi tên hiển thị tsudev-files: xong mã, chờ CI và merge

- **Mã phiếu**: 20261109-04
- **Từ**: claude - **Đến**: phiên sau
- **Thời điểm**: 09/11/2026 (cuối phiên, chủ dự án đóng phiên để tiết kiệm ngữ cảnh)
- **Trạng thái**: CHỜ NHẬN
- **Nhánh git**: `feat/doi-ten-hien-thi` (chưa push), `chore/khoi-tao-fork` (đã push, PR #1)

## 0. Ủy quyền của chủ dự án (đọc trước)

Chủ dự án đã ủy quyền cho agent tự làm mọi bước tới khi xong 100%, gồm push, mở PR, merge PR, cập nhật log.
Lời gốc: "tôi đồng ý ủy quyền cho phép bạn tự làm tất cả cho đến khi hoàn thành (bao gồm cấp toàn quyền cho phép bạn tự
động thực hiện tất cả các bước cần thiết tiếp theo cho đến khi hoàn thành 100% task/kế hoạch/phase...)". Lời dặn lúc
đóng phiên: agent phiên sau tự merge và thao tác mọi thứ tới khi xong kế hoạch.

Ủy quyền KHÔNG bao gồm: chuyển repo sang `tsudev-org` (mục 2.6, cần chủ dự án xác nhận trực tiếp), phát hành bản
`v2.4.0-vn.1` (cần máy Windows và khóa ký của chủ dự án), tắt GitHub Actions hay nới quyền bảo mật.

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
- [ ] 2.6. Chuyển repo sang `tsudev-org`: **KHÔNG làm** khi chưa có xác nhận trực tiếp của chủ dự án trong chính phiên
      đó (yêu cầu đến từ phiên khác `tsudev-b1`, chưa phải lời chủ dự án). Khi có xác nhận: làm đúng phiếu
      `logs/handover/20261109-03_yeu-cau-chuyen-repo-vao-tsudev-org.md` (gồm phần bổ sung cuối phiếu: thay
      gitleaks-action bằng binary 8.30.1 ghim SHA-256 tự đối chiếu với `gitleaks_8.30.1_checksums.txt`; KHÔNG bật CodeQL
      default setup; bật lại secret scanning + push protection; `git remote set-url origin
      https://github.com/tsudev-org/tsudev-files.git`; commit riêng đổi `RepositoryUrl` trong `ForkDefaults.cs`,
      `Directory.Build.props`, `SetupInfo.cs` và test, trước bản phát hành đầu tiên).
- [ ] 2.7. Cuối phiên: cập nhật `logs/STATE.md` (chuyển task sang "Đã hoàn thành"), điền mục 7 phiếu này, đóng phiếu
      bằng trạng thái HOÀN THÀNH, commit log (`chore(logs): ...`) qua PR nhỏ hoặc cùng PR #2 nếu còn mở.

## 3. File liên quan / đang khóa

| Đường dẫn | Lý do | Còn khóa? |
| --- | --- | --- |
| (toàn bộ khóa đổi tên đã nhả) | | Không |

Phiên sau ghi khóa mới vào `logs/LOCKS.md` trước khi sửa file, nhả ngay khi xong.

## 4. Yêu cầu gửi agent đang giữ khóa (nếu có)

- Không có.
- Phiên `tsudev-b1` (repo `tsudev`) đang chờ phản hồi về việc chuyển repo: khi chủ dự án xác nhận hoặc từ chối, báo lại
  qua ListAgents -> SendMessage và đóng phiếu `20261109-03`.

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

-
