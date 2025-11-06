# C1 60 - GuildWarRequestResult (by server)

## Is sent when

A guild master requested a guild war against another guild.

## Causes the following actions on the client side

The guild master of the other guild gets this request.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x60  | Packet header - packet type identifier |
| 3 | 1 | RequestResult |  | Result |

### RequestResult Enum

Describes the result of the guild war request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | GuildNotFound | Failed, guild not found. |
| 1 | RequestSentToGuildMaster | Request has been sent to the guild master. Waiting for his answer. |
| 2 | GuildMasterOffline | Failed, because the guild master is offline. |
| 3 | NotInGuild | Failed, because player is not in a guild. |
| 4 | Failed | The guild war couldn't be started, e.g. because the soccer arena is already in use. |
| 5 | NotTheGuildMaster | Failed, because player is not the guild master. |
| 6 | AlreadyInWar | Failed, because the requested guild is already in a war. |