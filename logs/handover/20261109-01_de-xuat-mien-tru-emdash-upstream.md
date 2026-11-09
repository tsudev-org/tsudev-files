# PHIẾU BÀN GIAO - Đề xuất lên tsudev-standards: miễn trừ em-dash cho mã kế thừa từ upstream

- **Mã phiếu**: 20261109-01
- **Từ**: claude (phiên tsudev-files) - **Đến**: chủ dự án, để mở Issue/PR tại `tsudev-tsudev/tsudev-standards`
- **Thời điểm**: 15:25 09/11/2026
- **Trạng thái**: MỞ
- **Nhánh git**: `chore/khoi-tao-fork` (repo này không cần sửa gì thêm)

## 1. Việc đã làm xong

- Đo đạc: `git grep -lP '\x{2014}'` (ngoài `.standards/`) ra 49 file, **đúng bằng** số file ở commit upstream
  `64890f1e`, tức fork không thêm em-dash nào; toàn bộ là mã/tài liệu kế thừa từ `KnifeLemon/Filee`.
- Bản nháp đề xuất ở mục 4 dưới đây, dán thẳng vào `proposals/` hoặc Issue của repo trung tâm được.
- Không sửa `.standards/` hay `scripts/check-standards.sh` (thuộc bộ quy ước, chỉ sửa ở trung tâm).

## 2. Việc dang dở + bước tiếp theo CỤ THỂ

- [ ] Chủ dự án mở Issue/PR ở `tsudev-standards` với nội dung mục 4 (agent không tự đẩy sang repo khác; bản
      `~/projects/tsudev-standards` trên máy này không phải repo git nên không tạo nhánh ở đó được).
- [ ] Khi trung tâm phát hành bản có cơ chế miễn trừ: chạy `scripts/sync-standards.sh` lên nhãn mới, tạo file
      danh sách miễn trừ theo đúng định dạng được duyệt, chạy `bash scripts/check-standards.sh` phải ra "đạt".

## 3. File liên quan / đang khóa

| Đường dẫn | Lý do | Còn khóa? |
| --- | --- | --- |
| (không) | Chỉ là đề xuất | Không |

## 4. Nội dung đề xuất (dán sang repo trung tâm)

**Tiêu đề**: Miễn trừ mục 7.1 (em-dash) cho file kế thừa nguyên trạng từ upstream ở repo fork

**Bối cảnh**: `tsudev-files` là fork của `KnifeLemon/Filee` (MIT, đang phát triển rất nhanh). 49 file kế thừa có
em-dash (U+2014), gồm cả dữ liệu test cố ý chứa ký tự ngoài Latin-1. Sửa chúng thì mỗi lần
`git merge upstream/main` sẽ xung đột ở hàng chục file và làm yếu test. Hiện cổng kiểm mục 4 luôn đỏ, nên cổng
kiểm mất tác dụng chặn với cả mã MỚI của fork.

**Đề xuất cơ chế** (thêm ngoại lệ thứ ba ở AGENTS.md mục 7.1 và `check-standards.sh` mục 4):

1. Repo con được phép có file `.standards-upstream-exempt` ở gốc, do repo con sở hữu (không nằm trong `.standards/`).
2. Dòng đầu bắt buộc khai báo nguồn: `upstream=<owner>/<repo>@<commit>`; các dòng sau là **đường dẫn file cụ thể**
   (không cho glob, không cho thư mục), mỗi dòng một file.
3. Cổng kiểm bỏ qua em-dash/en-dash trong đúng các file đó, nhưng in **LƯU Ý** kèm số file được miễn, để không bị
   quên.
4. Chống lạm dụng: cổng kiểm báo **TRƯỢT** nếu một file trong danh sách có dòng chứa em-dash mà dòng đó không tồn tại ở
   `<commit>` upstream đã khai (kiểm bằng `git show <commit>:<path>` khi commit có trong repo; không có thì chỉ
   kiểm số lượng). Nghĩa là: em-dash kế thừa thì được miễn, em-dash do fork tự thêm vẫn đỏ.
5. File mới của fork không bao giờ được vào danh sách (vì không tồn tại ở upstream).

**Tiêu chí hoàn thành**: ở `tsudev-files`, sau khi đồng bộ bản mới và thêm danh sách 49 file, `check-standards.sh`
đạt; thêm thử một em-dash vào mã mới của fork thì cổng kiểm đỏ.

## 5. Cảnh báo và quyết định quan trọng

- Quyết định gốc ghi ở `logs/STATE.md` mục "Quyết định quan trọng" (09/11/2026, em-dash trong mã kế thừa).
- Lấy lại danh sách file: `git grep -lP '\x{2014}' -- . ':!.standards'` (CHANGELOG.md, CONTRIBUTING.md và
  file trong `src/ tests/ build/ docs/ engines/ installer/`).

## 6. Trạng thái cổng kiểm

- [ ] `./scripts/check-standards.sh` đạt - CHƯA: 48 vi phạm em-dash, đều là mã kế thừa (lý do ở mục 4)
- [x] Lint và test đạt (xem phiếu 20261109-02)
- [x] `git status` sạch ngoài phạm vi bàn giao

## 7. Kết quả xử lý (agent nhận điền sau khi thực hiện)

-
