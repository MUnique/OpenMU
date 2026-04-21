# C1 02 - WhisperMessage (client gửi)

## Được gửi khi nào

Một người chơi gửi tin nhắn trò chuyện riêng tư đến một người chơi mục tiêu cụ thể.

## Hành động phía server

Tin nhắn được chuyển tiếp đến người chơi mục tiêu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte |  | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x02 | Tiêu đề gói - mã định danh loại gói |
| 3 | 10 | String |  | Tên người nhận |
| 13 |  | String |  | Tin nhắn |