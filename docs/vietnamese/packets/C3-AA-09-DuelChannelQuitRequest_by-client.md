# C3 AA 09 - DuelChannelQuitRequest (client gửi)

## Được gửi khi nào

Một người chơi đã yêu cầu rời khỏi trận đấu với tư cách là một khán giả.

## Hành động phía server

Máy chủ sẽ loại bỏ người chơi làm khán giả.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC3 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0xAA | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte | 0x09 | Tiêu đề gói - mã định danh loại gói phụ |