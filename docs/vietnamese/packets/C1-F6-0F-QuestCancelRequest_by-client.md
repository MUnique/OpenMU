# C1 F6 0F - QuestCancelRequest (client gửi)

## Được gửi khi nào

Máy khách trò chơi yêu cầu hủy một nhiệm vụ đang hoạt động.

## Hành động phía server

Máy chủ kiểm tra xem nhiệm vụ có đang được tiến hành hay không. Trong trường hợp này, trạng thái nhiệm vụ được đặt lại và phản hồi (F60F) được gửi lại cho máy khách.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 8 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF6 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x0F | Tiêu đề gói - mã định danh loại gói phụ |
| 4 | 2 | ShortLittleEndian |  | Số nhiệm vụ |
| 6 | 2 | ShortLittleEndian |  | Nhóm nhiệm vụ |