# C1 AA 07 - DuelInit (by server)

## Is sent when

When the duel starts.

## Causes the following actions on the client side

The client initializes the duel state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   30   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Result |
| 5 | 1 | Byte |  | RoomIndex |
| 6 | 10 | String |  | Player1Name |
| 16 | 10 | String |  | Player2Name |
| 26 | 2 | ShortBigEndian |  | Player1Id |
| 28 | 2 | ShortBigEndian |  | Player2Id |