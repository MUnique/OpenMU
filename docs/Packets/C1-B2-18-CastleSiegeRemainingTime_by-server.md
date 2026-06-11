# C1 B2 18 - CastleSiegeRemainingTime (by server)

## Is sent when

The server sends the remaining siege time.

## Causes the following actions on the client side

The client updates the remaining siege time display.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x18  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Hour |
| 5 | 1 | Byte |  | Minute |