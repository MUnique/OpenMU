# C1 53 - GuildKickResponse (by server)

## Is sent when

After a guild master sent a request to kick a player from its guild and the server processed this request.

## Causes the following actions on the client side

The client shows a message depending on the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x53  | Packet header - packet type identifier |
| 3 | 1 | GuildKickSuccess |  | Result |

### GuildKickSuccess Enum

The result of the guild kick request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failed | The kick request failed. |
| 1 | KickSucceeded | The kick request was successful. |
| 2 | GuildDisband | The guild has been disbanded. |