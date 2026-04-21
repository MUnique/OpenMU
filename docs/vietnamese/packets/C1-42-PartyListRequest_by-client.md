# C1 42 - PartyListRequest (client gửi)

## Được gửi khi nào

Khi người chơi mở menu nhóm trong client trò chơi.

## Hành động phía server

Nếu người chơi ở trong một nhóm, máy chủ sẽ gửi lại danh sách chứa thông tin về tất cả người chơi trong nhóm.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 3 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x42 | Tiêu đề gói - mã định danh loại gói |