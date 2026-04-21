# C1 F6 1B - QuestStateRequest (client gửi)

## Được gửi khi nào

Ứng dụng khách trò chơi yêu cầu trạng thái của một nhiệm vụ đang hoạt động cụ thể.

## Hành động phía server

Trạng thái nhiệm vụ được gửi lại (F61B) mà không thay đổi bất kỳ trạng thái nào nếu nhiệm vụ hiện đang được tiến hành.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x1B | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Số nhiệm vụ |
| 6 | 2 | ShortLittleEndian |  | Nhóm nhiệm vụ |