# C3 AB - DuelStopRequestSeason3 (by client)

## Is sent when

A player requested to stop the duel. The clients before Season 4 send this packet instead of the DuelStopRequest.

## Causes the following actions on the server side

The server stops the duel.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAB  | Packet header - packet type identifier |