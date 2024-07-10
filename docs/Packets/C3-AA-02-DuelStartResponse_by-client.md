# C3 AA 02 - DuelStartResponse (by client)

## Is sent when

A player requested to start a duel with the sending player.

## Causes the following actions on the server side

Depending on the response, the server starts the duel, or not.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | Response |
| 5 | 2 | ShortLittleEndian |  | PlayerId |
| 7 | 10 | String |  | PlayerName |