# PHIẾU YÊU CẦU - Áp dụng ngoại lệ upstream (quy ước v3.3.0) để cổng em-dash đạt

- **Mã phiếu**: 20261110-01
- **Từ**: phiên tsudev-lotus (projects-55) - **Đến**: phiên tsudev-files (tsudev-files-50) / phiên sau của repo này
- **Thời điểm**: 10/11/2026
- **Trạng thái**: MỞ - chờ `tsudev-standards` PR #48 được merge và gắn nhãn `v3.3.0`
- **Thay thế**: phiếu `20261109-01_de-xuat-mien-tru-emdash-upstream.md` (đề xuất theo danh sách file). Cơ chế được
  chọn ở PR #48 miễn **theo dòng**, không cần liệt kê 48 file, và chống khai `ref` giả.

## Cơ chế (tóm tắt, chi tiết: `.standards/AGENTS.md` mục 7.1 ngoại lệ 3 sau khi đồng bộ)

- File `.standards-upstream` ở gốc repo: `repo=KnifeLemon/Filee`, `ref=<SHA đủ 40 ký tự upstream đã gộp>`.
- Dòng có em-dash chỉ được miễn nếu có NGUYÊN VĂN ở commit `ref`; `ref` phải là tổ tiên HEAD; CI chạy
  `STANDARDS_VERIFY_UPSTREAM=1` để xác minh `ref` thuộc github.com/KnifeLemon/Filee.

## Đo thử (bản clone tạm, nhánh `chore/repo-url-tsudev-org` @ `af4a9e28`, không sửa repo này)

- `ref=64890f1e3fdc3c581f6995c1ca849d47bbed339c`, xác minh trực tuyến: đạt.
- Miễn 295 dòng kế thừa. **Còn 1 vi phạm thật do fork tạo ra**:
  `src/Filee.App/Assets/i18n/ru.json:1348` (`installer.error.incomplete`) - fork đổi URL sang tsudev-org nhưng giữ
  em-dash, nên dòng không còn nguyên văn upstream. Sửa: thay em-dash (U+2014, có khoảng trắng hai bên) bằng ` - ` trong đúng dòng đó.

## Việc cần làm (sau khi có nhãn v3.3.0)

1. `./scripts/sync-standards.sh --ref v3.3.0`
2. `printf 'repo=KnifeLemon/Filee\nref=64890f1e3fdc3c581f6995c1ca849d47bbed339c\n' > .standards-upstream`
   (nếu đã gộp upstream mới hơn thì dùng SHA đó).
3. Sửa `ru.json:1348` như trên.
4. CI: chạy cổng kiểm với `env: STANDARDS_VERIFY_UPSTREAM: '1'`; bỏ mọi `continue-on-error` của bước này nếu có.
5. `STANDARDS_VERIFY_UPSTREAM=1 ./scripts/check-standards.sh` phải ĐẠT mục 4. Ghi kết quả vào `logs/STATE.md`, đóng
   phiếu `20261109-01`.

Lưu ý thêm: mẫu `website/scripts/check-dist.mjs` của nhánh website có em-dash literal trong regex
(en-dash U+2013 và em-dash U+2014 viết thẳng trong regex); `tsudev-lotus` đã đổi sang `/[\u2013\u2014]/`. Nên đổi tương tự (dòng đó không phải mã upstream nên
không được miễn).

## Kết quả xử lý

-

> Ghi chú phiên tsudev-files (10/11/2026): khi commit phiếu này, các ký tự en/em-dash viết thẳng trong phiếu được đổi
> sang dạng mã (U+2013/U+2014) để `check-standards.sh` không tính thêm vi phạm; nội dung không đổi.
