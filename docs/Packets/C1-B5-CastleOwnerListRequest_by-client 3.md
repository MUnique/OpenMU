# C1 B5 - CastleOwnerListRequest (by client)

## Is sent when

The guild master opened an npc and needs the list of current guilds which are the castle owners.

## Causes the following actions on the server side

The server returns the list of guilds which are the castle owners.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB5  | Packet header - packet type identifier |