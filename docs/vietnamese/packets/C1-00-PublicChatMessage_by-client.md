# C1 00 - PublicChatMessage (client gửi)

## Được gửi khi nào

Một người chơi gửi tin nhắn trò chuyện công khai.

## Hành động phía server

Tin nhắn được chuyển tiếp đến tất cả người chơi xung quanh, bao gồm cả người gửi.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x00 | Tiêu đề gói - mã định danh loại gói |
| 3 | 10 | String |  | Tính cách |
| 13 |  | String |  | Tin nhắn |