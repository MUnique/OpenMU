# C1 AA 05 - DuelHealthUpdate (by server)

## Is sent when

When the health/shield of the duel players has been changed.

## Causes the following actions on the client side

The client updates the displayed health and shield bars.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | Player1Id |
| 6 | 2 | ShortBigEndian |  | Player2Id |
| 8 | 1 | Byte |  | Player1HealthPercentage |
| 9 | 1 | Byte |  | Player2HealthPercentage |
| 10 | 1 | Byte |  | Player1ShieldPercentage |
| 11 | 1 | Byte |  | Player2ShieldPercentage |