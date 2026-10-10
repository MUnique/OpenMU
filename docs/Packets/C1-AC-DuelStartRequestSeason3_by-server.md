# C1 AC - DuelStartRequestSeason3 (by server)

## Is sent when

After another client sent a DuelStartRequestSeason3, to ask the requested player for a response. It's the packet for the clients before Season 4.

## Causes the following actions on the client side

The client shows the duel request.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   15   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAC  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | RequesterId |
| 5 | 10 | String |  | RequesterName |