# C1 61 - GuildWarRequest (by server)

## Is sent when

A guild master requested a guild war against another guild.

## Causes the following actions on the client side

The guild master of the other guild gets this request.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x61  | Packet header - packet type identifier |
| 3 | 8 | String |  | GuildName |
| 11 | 1 | GuildWarType |  | Type |

### GuildWarType Enum

Describes the type of the guild war.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Normal | A normal guild war. |
| 1 | Soccer | A guild soccer match. |