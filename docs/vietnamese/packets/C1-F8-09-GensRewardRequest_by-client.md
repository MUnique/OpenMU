# C1 F8 09 - GensRewardRequest (client gửi)

## Được gửi khi nào

Khách hàng trò chơi yêu cầu nhận phần thưởng từ gens npc.

## Hành động phía server

Máy chủ kiểm tra xem người chơi có đủ điểm để nhận phần thưởng hay không và gửi phản hồi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xF8 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x09 | Tiêu đề gói - mã định danh loại gói phụ |
| 3 | 1 | GensType |  | GensType |

### Enum GensType
Mô tả kiểu gen.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại gen không xác định. |
| 1 | Duprian | Thị tộc Duprian. |
| 2 | Vanert | Gia đình Vanert. |