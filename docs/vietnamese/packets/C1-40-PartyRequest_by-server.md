# C1 40 - PartyRequest (server gửi)

## Được gửi khi nào

Khi một người chơi khác gửi lời mời party tới người nhận packet này.

## Hành động phía client

Hiển thị lời mời party.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 5 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0x40 | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | RequesterId |
