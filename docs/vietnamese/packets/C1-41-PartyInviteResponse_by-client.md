# C1 41 - PartyInviteResponse (client gửi)

## Được gửi khi nào

Khi người chơi nhận lời mời party từ người chơi khác và gửi phản hồi.

## Hành động phía server

Nếu người gửi phản hồi chấp nhận lời mời, server thêm người đó vào party.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 6 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x41 | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Accepted |
| 4 | 2 | ShortBigEndian |  | RequesterId |
