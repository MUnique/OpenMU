# C1 91 - MiniGameOpeningStateRequest (client gửi)

## Được gửi khi nào

Người chơi yêu cầu lấy thời gian còn lại của sự kiện hiện đang tham gia.

## Hành động phía server

Thời gian còn lại sẽ được gửi lại cho khách hàng.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x91 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | MiniGameType |  | Loại sự kiện |
| 4 | 1 | Byte |  | Cấp độ sự kiện |

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