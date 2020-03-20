# C1 52 - GuildListRequest (by client)

## Is sent when

A guild player opens its guild menu in the game client.

## Causes the following actions on the server side

A list of all guild members and their state is sent back as response.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x52  | Packet header - packet type identifier |