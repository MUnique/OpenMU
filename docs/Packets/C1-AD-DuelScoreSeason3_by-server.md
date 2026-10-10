# C1 AD - DuelScoreSeason3 (by server)

## Is sent when

When the score of the duel has been changed. It's the packet for the clients before Season 4.

## Causes the following actions on the client side

The client updates the displayed duel score.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAD  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | Player1Id |
| 5 | 2 | ShortBigEndian |  | Player2Id |
| 7 | 1 | Byte |  | Player1Score |
| 8 | 1 | Byte |  | Player2Score |