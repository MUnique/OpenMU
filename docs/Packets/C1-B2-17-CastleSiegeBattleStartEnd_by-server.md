# C1 B2 17 - CastleSiegeBattleStartEnd (by server)

## Is sent when

The server notifies all players that the castle siege battle has started or ended.

## Causes the following actions on the client side

The client updates the siege state accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x17  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | IsStarted |