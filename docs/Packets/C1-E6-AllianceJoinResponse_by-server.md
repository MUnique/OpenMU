# C1 E6 - AllianceJoinResponse (by server)

## Is sent when

After processing an alliance request. This message is sent back to the requesting guild master.

## Causes the following actions on the client side

The requester gets a corresponding message showing the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE6  | Packet header - packet type identifier |
| 3 | 1 | AllianceJoinResult |  | Result |

### AllianceJoinResult Enum

The result of an alliance operation.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NotInGuild | The player is not in a guild. |
| 1 | NotTheGuildMaster | The player is not the guild master. |
| 2 | GuildNotFound | The target guild was not found. |
| 3 | GuildMasterOffline | The guild master is offline. |
| 4 | RequestSent | The request was sent successfully. |
| 5 | Success | The alliance was created successfully. |
| 6 | Failed | The operation failed. |
| 7 | HasHostility | The guild has a hostility relationship. |
| 8 | AllianceFull | The alliance is full. |
| 9 | AlreadyInAlliance | The guild is already in an alliance. |
| 10 | Removed | The guild was removed from the alliance. |