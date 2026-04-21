# C1 40 - PartyInviteRequest (client gửi)

## Được gửi khi nào

Người chủ nhóm muốn mời một người chơi khác vào nhóm của mình.

## Hành động phía server

Nếu người chơi yêu cầu không có nhóm hoặc là chủ nhóm, yêu cầu sẽ được gửi đến người chơi mục tiêu.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x40 | Tiêu đề gói - mã định danh loại gói |
| 3 | 2 | ShortBigEndian |  | Id người chơi mục tiêu |