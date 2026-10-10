# C3 AA - DuelStartRequestSeason3 (by client)

## Is sent when

The player requests to start a duel with another player. The clients before Season 4 send this packet, which has no sub code, instead of the DuelStartRequest.

## Causes the following actions on the server side

The server sends a DuelStartRequestSeason3 to the other player.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   15   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | PlayerId |
| 5 | 10 | String |  | PlayerName |