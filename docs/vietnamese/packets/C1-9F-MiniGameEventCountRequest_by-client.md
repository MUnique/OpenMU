# C1 9F - MiniGameEventCountRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu lấy số lần nhập của trò chơi nhỏ được chỉ định.

## Hành động phía server

Thời gian còn lại sẽ được gửi lại cho khách hàng. Tuy nhiên, nó không thực sự được xử lý trên các nguồn máy chủ đã biết.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x9F | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | MiniGameType |  | trò chơi nhỏ |

### Enum MiniGameType
Xác định loại trò chơi nhỏ.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | Loại trò chơi nhỏ không xác định. |
| 1 | DevilSquare | Trò chơi mini hình vuông ma quỷ. |
| 2 | BloodCastle | Game mini lâu đài máu. |
| 3 | CursedTemple | Mini game ngôi đền bị nguyền rủa |
| 4 | ChaosCastle | Game mini lâu đài hỗn loạn. |
| 5 | IllusionTemple | Game mini ngôi đền ảo ảnh. |
| 6 | Doppelganger | Trò chơi mini doppelgänger. |