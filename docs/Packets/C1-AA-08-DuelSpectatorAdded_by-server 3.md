# C1 AA 08 - DuelSpectatorAdded (by server)

## Is sent when

When a spectator joins a duel.

## Causes the following actions on the client side

The client updates the list of spectators.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   14   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x08  | Packet header - sub packet type identifier |
| 4 | 10 | String |  | Name |