# C1 F6 1A - ActiveQuestListRequest (client gửi)

## Được gửi khi nào

Khách hàng yêu cầu trạng thái của tất cả các nhiệm vụ, thường là sau khi vào trò chơi.

## Hành động phía server

Danh sách nhiệm vụ đang hoạt động được gửi lại (F61A) mà không thay đổi bất kỳ trạng thái nào. Danh sách này chỉ chứa tất cả các nhiệm vụ đang chạy hoặc đã hoàn thành cho mỗi nhóm.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x1A | Tiêu đề gói - mã định danh loại gói phụ |