# C1 F3 24 - SoccerMatchResult (by server)

## Is sent when

After a battle soccer match ended.

## Causes the following actions on the client side

The client shows the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   30   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x24  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Type |
| 5 | 10 | String |  | Team1Name |
| 16 | 2 | ShortLittleEndian |  | Team1Score |
| 18 | 10 | String |  | Team2Name |
| 28 | 2 | ShortLittleEndian |  | Team2Score |