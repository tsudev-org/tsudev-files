# PHIẾU YÊU CẦU - Chuyển repo `tsudev-files` sang Organization `tsudev-org`

- **Mã phiếu**: 20261109-03
- **Từ**: phiên tsudev-b1 (repo `tsudev`) - **Đến**: phiên đang làm ở repo này
- **Thời điểm**: 16:33 09/11/2026
- **Trạng thái**: HOÀN THÀNH (10/11/2026) - repo ở `tsudev-org/tsudev-files`, Public; kết quả đo ở cuối phiếu
- **Uỷ quyền**: chủ dự án đồng ý chuyển repo này vào `tsudev-org` (chat 09/11/2026). Giao cho phiên của repo này vì
  đang có việc dở ở đây - phiên khác chuyển hộ có thể làm hỏng việc đang chạy.

## Vì sao làm

Org `tsudev-org` đã tạo 09/11. Năm repo đã chuyển cùng ngày và nghiệm thu: `tsudev`, `tsudev-cwico`,
`tsudev-swico`, `tsudev-contact`, `tsudev-standards`. Còn repo này và repo anh em (`tsudev-files`/`tsudev-trafficmonitor`).
Org đã cài sẵn GitHub App `render` và `gitguardian` (All repositories) - không phải cài lại.

## Các bước (làm khi KHÔNG có push/PR/release nào đang chạy)

1. Ghi khoá vào `logs/LOCKS.md`. Chụp hiện trạng TRƯỚC khi chuyển:
   - `gh api repos/tsudev-tsudev/tsudev-files` → `visibility`, `security_and_analysis`
   - `gh api repos/tsudev-tsudev/tsudev-files/branches/main/protection` (repo private gói Free sẽ 403 - bình thường)
   - tên secret: `gh api repos/tsudev-tsudev/tsudev-files/actions/secrets`; environment; số release
   - `gh api repos/tsudev-tsudev/tsudev-files/code-scanning/default-setup` (`configured` hay không)
   - mã HTTP của mọi URL mà bản app ĐÃ CÀI gọi tới (updater, link tải) - dùng `curl -sIL -o /dev/null -w '%{http_code}'`
2. Chuyển: `gh api -X POST repos/tsudev-tsudev/tsudev-files/transfer -f new_owner=tsudev-org`, rồi chờ
   `gh api repos/tsudev-org/tsudev-files` trả về (vài giây). Token `gh` sẵn có đủ quyền.
3. ⚠️ **Chuyển repo TẮT secret scanning + push protection** (đo được ở cả 4 repo public) **và CodeQL default setup**.
   Bật lại:
   ```bash
   gh api -X PATCH repos/tsudev-org/tsudev-files --input - <<'J'
   {"security_and_analysis":{"secret_scanning":{"status":"enabled"},"secret_scanning_push_protection":{"status":"enabled"}}}
   J
   gh api -X PATCH repos/tsudev-org/tsudev-files/code-scanning/default-setup -f state=configured -f query_suite=default  # chỉ khi trước đó configured; KHÔNG truyền languages
   ```
4. Đối chiếu với ảnh chụp bước 1: protection, secret, release, mọi URL cũ vẫn 200 (GitHub tự chuyển hướng).
5. `git remote set-url origin https://github.com/tsudev-org/tsudev-files.git` rồi `git fetch`.
6. Mở một PR thử: số check phải bằng trước khi chuyển (GitGuardian phải có). Chính sách Actions của Org có thể khác
   tài khoản cá nhân - workflow nào bị chặn thì báo chủ dự án, đừng nới quyền tự ý.

## ⚠️ Riêng repo này

- Repo đang khoá `src/Filee.App/Services/UpdateService.cs` và `src/Filee.Core/ForkDefaults.cs` cho việc đổi tên hiển thị -
  hai file đó ghim `RepositoryUrl = https://github.com/tsudev-tsudev/tsudev-files`. Làm xong việc đổi tên đang dở TRƯỚC,
  rồi mới chuyển repo; đổi URL sang `tsudev-org` là một commit riêng sau khi chuyển.
- Repo chưa có release nào (đo 09/11) ⇒ chưa có bản đã cài phụ thuộc URL cũ: nên đổi URL trong mã sang `tsudev-org`
  TRƯỚC bản phát hành đầu tiên.
- Repo Private, gói Free: branch protection/ruleset không đọc được (403) - bình thường, không phải lỗi do chuyển.

## ⛔ Quy tắc vĩnh viễn

**KHÔNG BAO GIỜ tạo repo, fork hay đổi tên repo nào thành `tsudev-files` (hay bất kỳ tên repo cũ nào) dưới tài khoản
`tsudev-tsudev`.** GitHub bỏ chuyển hướng ngay khi tên cũ bị chiếm lại ⇒ mọi URL cũ (updater trong bản đã cài,
winget, link tài liệu, `git remote`) gãy IM LẶNG hoặc trỏ sang repo sai. Repo mới cho hệ tsudev tạo trong `tsudev-org`.

## Báo lại

Xong thì đóng phiếu này (HOÀN THÀNH + kết quả đo), ghi vào `logs/STATE.md` của repo này. Phiếu gốc phía tsudev:
`tsudev/logs/handover/20261109-03_phien-44-chuyen-repo-vao-tsudev-org.md`.

## Bổ sung từ phiên tsudev-b1 (sau 16:33 09/11/2026), phiên tsudev-files ghi nhận

- Trước khi chuyển: thay `gitleaks/gitleaks-action` trong `.github/workflows/security.yml` bằng binary ghim phiên bản
  + SHA-256 (action đòi `GITLEAKS_LICENSE` khi repo thuộc Organization). Mẫu: `tsudev/.github/workflows/security-scan.yml`
  job `gitleaks` (8.30.1). Phiên tsudev-files sẽ tự đối chiếu SHA-256 với `gitleaks_8.30.1_checksums.txt` của bản
  phát hành chính thức, không chép nguyên con số từ tin nhắn.
- Bước 3: repo này CÓ workflow CodeQL riêng (job `codeql` trong `security.yml`, advanced) ⇒ KHÔNG bật CodeQL default
  setup. Sau khi chuyển: `gh api repos/tsudev-org/tsudev-files/actions/workflows` - mọi workflow phải `active`.
- Trạng thái: chủ dự án đã xác nhận trực tiếp trong phiên tsudev-files (09/11/2026). Kế hoạch chi tiết: phiếu
  `20261109-04` bước 2.6.

## Kết quả (phiên tsudev-files, 09-10/11/2026) - HOÀN THÀNH

Đã chuyển `tsudev-tsudev/tsudev-files` -> `tsudev-org/tsudev-files` và đổi sang Public (chủ dự án ủy quyền). Số đo
đầy đủ: phiếu `20261109-05` mục 1.

| Hạng mục | Trước (tsudev-tsudev, Private) | Sau (tsudev-org, Public) |
| --- | --- | --- |
| Secret / environment / release | 0 / 0 / 0 | 0 / 0 / 0 |
| Workflow `active` | 10/10 | 10/10 |
| PR mở | 15 | 15 |
| Secret scanning, push protection | không có (Private Free) | `enabled` (bật lại sau khi chuyển) |
| CodeQL default setup | 403 | `not-configured` (đúng ý: có workflow CodeQL riêng) |
| Branch protection | 403 (Private Free) | chưa đặt (đề xuất ruleset, chờ chủ dự án) |
| URL cũ `github.com/tsudev-tsudev/tsudev-files` (+ `/releases`, `.git/info/refs`) | 200 | 200 qua chuyển hướng |

- PR thử: CI chạy đủ, có GitGuardian; CodeQL (C#) và `build (win-arm64)` giờ CHẠY (trước bị bỏ qua vì Private).
- gitleaks dùng binary 8.30.1 ghim SHA-256 (PR #17) trước khi chuyển. `RepositoryUrl` và mọi URL đổi sang
  `tsudev-org` ở PR #21 (gộp 10/11/2026 07:50Z). `git grep tsudev-tsudev` chỉ còn tên người (AGENTS.md, ADR-0001).
- Phiên `tsudev-b1` không còn (09/11) nên chỉ ghi kết quả ở đây.
