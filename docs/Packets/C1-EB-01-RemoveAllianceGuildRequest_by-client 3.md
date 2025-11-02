# C1 EB 01 - RemoveAllianceGuildRequest (by client)

## Is sent when

An alliance guild master wants to remove a guild from the alliance.

## Causes the following actions on the server side

The server removes the guild from the alliance.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xEB  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 |  | String |  | GuildName |