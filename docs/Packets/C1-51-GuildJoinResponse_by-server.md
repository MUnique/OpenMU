# C1 51 - GuildJoinResponse (by server)

## Is sent when

After a guild master responded to a request of a player to join his guild. This message is sent back to the requesting player.

## Causes the following actions on the client side

The requester gets a corresponding message showing.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x51  | Packet header - packet type identifier |
| 3 | 1 | GuildJoinRequestResult |  | Result |

### GuildJoinRequestResult Enum

The result of the guild join request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Refused | Refused by the guild master. |
| 1 | Accepted | Accepted by the guild master. |
| 2 | GuildFull | The guild is full. |
| 3 | Disconnected | The guild master is disconnected. |
| 4 | NotTheGuildMaster | The requested player is not the guild master of its guild. |
| 5 | AlreadyHaveGuild | The player already has a guild. |
| 6 | GuildMasterOrRequesterIsBusy | he guild master or the requesting player is busy, e.g. by another request or by an ongoing guild war. |
| 7 | MinimumLevel6 | The requesting player needs a minimum level of 6. |