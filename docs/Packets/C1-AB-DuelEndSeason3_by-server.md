# C1 AB - DuelEndSeason3 (by server)

## Is sent when

After a duel ended. It's the packet for the clients before Season 4, which is sent to each of the two duelists with its own data.

## Causes the following actions on the client side

The client updates its state and closes the duel.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   15   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAB  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | PlayerId |
| 5 | 10 | String |  | PlayerName |