# C1 B2 16 - CastleSiegeOwnershipChangeNotification (by server)

## Is sent when

The server notifies all players that the castle ownership has changed.

## Causes the following actions on the client side

The client shows the new castle owner guild name.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x16  | Packet header - sub packet type identifier |
| 4 | 8 | String |  | GuildName |