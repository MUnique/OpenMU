# C1 AA 04 - DuelScore (by server)

## Is sent when

When the score of the duel has been changed.

## Causes the following actions on the client side

The client updates the displayed duel score.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   10   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | Player1Id |
| 6 | 2 | ShortBigEndian |  | Player2Id |
| 8 | 1 | Byte |  | Player1Score |
| 9 | 1 | Byte |  | Player2Score |