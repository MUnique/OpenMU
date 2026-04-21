# Tiến độ tính năng dự án

Để xem tiến độ tổng thể của dự án, tham khảo
[trang projects](https://github.com/MUnique/OpenMU/projects).

Tài liệu này theo dõi tiến độ triển khai packet handlers.
Mỗi packet handler thường cần logic gameplay phía sau để hoàn thiện.

## Quy ước độ phức tạp

- `1`: độ phức tạp thấp, effort thấp
- `10`: độ phức tạp cao, effort cao
- `0`: không có kế hoạch triển khai

## Cách đọc bảng gốc

File gốc có bảng lớn gồm:

- `Feature`: tên tính năng/nhóm packet
- `Packet code`: mã opcode
- `Progress`: phần trăm hoàn thành
- `Complexity`: độ phức tạp
- `Note`: ghi chú bổ sung

Bạn có thể xem trực tiếp bảng chi tiết tại file gốc:

- [Progress gốc](../Progress.md)

## Gợi ý sử dụng trong dự án của bạn

- Ưu tiên các packet `Progress 100%` để làm baseline tương thích client.
- Với packet `0%`, đánh giá lại có cần cho scope MVP hay không.
- Tập trung trước vào nhóm:
  - Login/Character/Map
  - Movement/Hit/Inventory
  - Packet cốt lõi để vào game ổn định
