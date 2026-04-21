# C1 AA 03 - DuelEnd (server gửi)

## Được gửi khi nào

Sau khi một trận duel kết thúc.

## Hành động phía client

Client cập nhật trạng thái duel.

## Cấu trúc

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte | 0xC1 | [Packet type](../../Packets/PacketTypes.md) |
| 1 | 1 | Byte | 17 | Packet header - độ dài packet |
| 2 | 1 | Byte | 0xAA | Packet header - packet type identifier |
| 3 | 1 | Byte | 0x03 | Packet header - sub packet type identifier |
| 4 | 1 | Byte | 0 | Result |
| 5 | 2 | ShortBigEndian |  | OpponentId |
| 7 | 10 | String |  | OpponentName |
