# C1 E2 - GuildTypeChangeRequest (by client)

## Is sent when

A guild master wants to change the type of its guild. Didn't find any place in the client where this is sent.

## Causes the following actions on the server side

The server changes the kind of the guild. We assume it's whether the guild should be the main guild of an alliance, or not. Shouldn't be handled, because this is constant for the lifetime of an alliance.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE2  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | GuildType; 0 = Common, 1 = Guard, FF = None. |