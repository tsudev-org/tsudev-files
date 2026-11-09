# AGENTS.md - tsudev-files

> **ĐỌC FILE NÀY ĐẦU TIÊN trong mọi phiên làm việc mới.**

## Phần A - Quy ước chung của hệ sinh thái (KHÔNG SỬA Ở ĐÂY)

Toàn bộ quy ước chung nằm trong bản sao chỉ-đọc tại
[`.standards/AGENTS.md`](.standards/AGENTS.md). `MUST` đọc file đó trước.

Bản quy ước repo này đang dùng: xem [`.standards-version`](.standards-version).

| Cần gì | Đọc file nào |
| --- | --- |
| Điểm vào, nguyên tắc chung | `.standards/AGENTS.md` |
| Quy trình phiên, khóa file, bàn giao | `.standards/docs/AGENT_PROTOCOL.md` |
| Bảo mật bắt buộc | `.standards/docs/SECURITY_BASELINE.md` |
| Quy tắc `.gitignore` | `.standards/docs/GITIGNORE_POLICY.md` |
| Nhánh, commit, PR, phát hành | `.standards/docs/GIT_WORKFLOW.md` |
| Đăng nhập, đăng ký, xác minh tài khoản | `.standards/docs/AUTH_AND_ACCOUNT.md` |
| Giao diện và token | `.standards/docs/DESIGN_SYSTEM.md` |
| Bảng bản ghi, bộ chọn số bản ghi | `.standards/docs/DATA_TABLE.md` |
| Logo, favicon, icon ứng dụng | `.standards/docs/BRAND_ASSETS.md` |
| tsudev.com, ảnh đại diện, trang hồ sơ | `.standards/docs/ECOSYSTEM_IDENTITY.md` |
| Cấu trúc thư mục | `.standards/docs/PROJECT_STRUCTURE.md` |
| Chọn ngôn ngữ, framework | `.standards/docs/LANGUAGE_SELECTION.md` |
| Hạ tầng 0 đồng | `.standards/docs/FREE_TIER_STACK.md` |
| Trình soạn thảo nội dung | `.standards/docs/RICH_TEXT_EDITOR.md` |
| Tìm kiếm và lọc tiếng Việt | `.standards/docs/SEARCH_AND_FILTER.md` |
| Kiểm thử và chất lượng mã | `.standards/docs/TESTING_QUALITY.md` |
| Khả năng truy cập | `.standards/docs/ACCESSIBILITY.md` |

`MUST NOT` sửa bất kỳ file nào trong `.standards/`. Cần đổi quy ước thì mở đề
xuất tại repo `tsudev-standards` theo `.standards/docs/SYNC.md` mục 1.

## Phần B - Riêng của repo này

### B.1. Repo này là gì

- **Loại**: phần mềm desktop (Windows/Linux/macOS) + công cụ dòng lệnh
- **Stack**: C# / .NET 10, Avalonia 12; fork của `KnifeLemon/Filee` (MIT). Kiến trúc gốc: `docs/ARCHITECTURE.md`;
  bảo mật: `docs/vi/KIEN-TRUC-BAO-MAT.md`
- **Mức phân loại dữ liệu cao nhất**: D3 (tệp người dùng xử lý cục bộ; khóa ký cập nhật)
- **Người liên hệ khi có sự cố**: tsudev-tsudev

### B.2. Chạy trong 5 phút

```bash
dotnet build Filee.slnx                      # .NET SDK 10 (global.json)
dotnet test tests/Filee.Core.Tests           # và tests/Filee.{App,Cli,Setup,Engines,Platform}.Tests
python3 -I tools/check_i18n_vi.py            # bản dịch tiếng Việt
dotnet run --project src/Filee.Cli -- help   # FILEE_LANG=en để xem tiếng Anh
```

### B.3. Ranh giới sở hữu file

| Vùng | Chủ sở hữu |
| --- | --- |
| `src/Filee.Core/ForkDefaults.cs`, `Directory.Build.props` (URL, khóa) | Chủ dự án - mọi thay đổi cần duyệt |
| `src/Filee.App/Assets/i18n/vi.json` | Agent/người dịch; giữ đồng bộ khóa với `en.json` |
| `src/Filee.Cli/` (chuỗi `CliLanguage.T`) | Agent dòng lệnh |
| `tools/`, `.github/workflows/security.yml`, `docs/vi/` | Riêng của fork |
| Phần còn lại của `src/`, `tests/`, `build/` | Mã upstream - sửa tối thiểu, ghi chú `tsudev-files:` tại chỗ sửa |

### B.4. Quy tắc riêng

- Remote `upstream` = KnifeLemon/Filee (chiều đẩy bị khóa). Gộp bằng `git fetch upstream && git merge upstream/main`.
- Mỗi chỗ sửa trong mã upstream đánh dấu bằng chú thích bắt đầu `tsudev-files:` để dễ tìm khi gộp.
- Không đổi khóa JSON đầu ra của dòng lệnh (script bên ngoài phụ thuộc).
- Không bao giờ đưa khóa bí mật ký cập nhật vào repo; quy trình ở `docs/vi/PHAT-HANH.md`.
- Ngoại lệ đã ghi nhận: em-dash trong mã kế thừa upstream (xem `logs/STATE.md`, mục Quyết định).
