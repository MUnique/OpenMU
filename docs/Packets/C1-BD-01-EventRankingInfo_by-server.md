# C1 BD 01 - EventRankingInfo (by server)

## Is sent when

When the event has ended.

## Causes the following actions on the client side

The client shows the score.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   28   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 3 | 10 | String |  | AccountId |
| 13 | 10 | String |  | GameId |
| 24 | 4 | IntegerLittleEndian |  | ServerCode |
| 28 | 4 | IntegerLittleEndian |  | Score |
| 32 | 4 | IntegerLittleEndian |  | Class |
| 36 | 4 | IntegerLittleEndian |  | SquareNumber |