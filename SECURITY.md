# Chính sách bảo mật - TsuFiles

## Phiên bản được hỗ trợ

Chỉ bản phát hành mới nhất tại <https://github.com/tsudev-org/tsufiles/releases/latest> nhận bản vá bảo mật.

## Báo cáo lỗ hổng

**Không mở issue công khai.** Dùng tính năng báo cáo riêng của GitHub:
<https://github.com/tsudev-org/tsufiles/security/advisories/new>

Vui lòng gửi kèm: phiên bản, hệ điều hành, tệp mẫu (nếu có thể chia sẻ) hoặc các bước tái hiện, và mức ảnh hưởng.

Cam kết phản hồi: xác nhận đã nhận trong **3 ngày làm việc**, đánh giá ban đầu trong **7 ngày**, bản vá cho lỗi
nghiêm trọng (thực thi mã, ghi tệp ngoài thư mục đích, vượt qua kiểm tra chữ ký cập nhật) sớm nhất có thể.

Lỗ hổng nằm trong mã kế thừa từ Filee gốc sẽ được báo tiếp lên upstream theo
[chính sách của họ](docs/SECURITY.upstream.md) sau khi bạn đồng ý.

## Phạm vi đặc biệt quan tâm

- Xử lý tệp không tin cậy (PDF, Office, HWP, ảnh, video, tệp nén, email) dẫn tới thực thi mã, đọc/ghi tệp ngoài ý muốn
  (path traversal, "zip slip"), tải tài nguyên mạng khi đang chuyển đổi, hoặc từ chối dịch vụ.
- Chuỗi cập nhật: tải, kiểm SHA-256, chữ ký ECDSA của SHA256SUMS.
- Tải engine tùy chọn (FFmpeg, LibreOffice, Pandoc, ...): kiểm SHA-256 ghim sẵn.

Mô hình mối đe dọa và các lớp phòng thủ: [docs/vi/KIEN-TRUC-BAO-MAT.md](docs/vi/KIEN-TRUC-BAO-MAT.md).
