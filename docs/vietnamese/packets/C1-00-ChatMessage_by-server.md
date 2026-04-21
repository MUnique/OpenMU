# C1 00 - ChatMessage (server gửi)

## Được gửi khi nào

Một người chơi gửi tin nhắn trò chuyện.

## Hành động phía client

Tin nhắn được hiển thị trong hộp trò chuyện và phía trên ký tự của người gửi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói |
| 2 | 1 | ChatMessageType |  | Type |
| 3 | 10 | String |  | Người gửi |
| 13 |  | String |  | Tin nhắn |

### Enum ChatMessageType
Xác định loại tin nhắn trò chuyện.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | Tin nhắn này là tin nhắn trò chuyện bình thường, ví dụ: công cộng, trong một đảng hoặc hội. |
| 2 | Whisper | Tin nhắn được gửi riêng cho người chơi nhận. |