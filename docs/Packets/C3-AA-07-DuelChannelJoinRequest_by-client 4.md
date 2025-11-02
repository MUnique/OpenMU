# C3 AA 07 - DuelChannelJoinRequest (by client)

## Is sent when

A player requested to join the duel as a spectator.

## Causes the following actions on the server side

The server will add the player as spectator.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x07  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ChannelId |