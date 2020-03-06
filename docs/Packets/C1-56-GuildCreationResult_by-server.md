# C1 56 - GuildCreationResult (by server)

## Is sent when

After a player requested to create a guild at the guild master NPC.

## Causes the following actions on the client side

Depending on the result, a message is shown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x56  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success |
| 4 | 1 | GuildCreationErrorType |  | Error |

### GuildCreationErrorType Enum

Defines a guild creation error.

| Value | Name | Description |
|-------|------|-------------|
| 0 | None | No error occured. |
| 179 | GuildNameAlreadyTaken | The requested guild name is already taken. |