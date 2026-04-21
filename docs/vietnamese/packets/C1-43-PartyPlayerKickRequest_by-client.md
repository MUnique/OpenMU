# C1 43 - PartyPlayerKickRequest (client gửi)

## Được gửi khi nào

Người chủ nhóm muốn đá một người chơi khác khỏi nhóm của mình hoặc khi một người chơi muốn đá chính mình khỏi nhóm của mình.

## Hành động phía server

Nếu người chơi gửi là chủ nhóm hoặc người chơi muốn tự đá mình, người chơi mục tiêu sẽ bị loại khỏi nhóm.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 4 | Tiêu đề gói - chiều dài của gói |
| 2 | 1 | Byte | 0x43 | Tiêu đề gói - mã định danh loại gói |
| 3 | 1 | Byte |  | Chỉ số người chơi |