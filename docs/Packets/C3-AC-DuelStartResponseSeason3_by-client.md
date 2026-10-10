# C3 AC - DuelStartResponseSeason3 (by client)

## Is sent when

A player requested to start a duel with the sending player. The clients before Season 4 send this packet instead of the DuelStartResponse.

## Causes the following actions on the server side

Depending on the response, the server starts the duel, or not.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAC  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Response |
| 4 | 2 | ShortBigEndian |  | PlayerId |
| 6 | 10 | String |  | PlayerName |