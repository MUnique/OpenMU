# C1 E1 - GuildRoleAssignRequest (by client)

## Is sent when

A guild master wants to change the role of a guild member.

## Causes the following actions on the server side

The server changes the role of the guild member.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   15   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE1  | Packet header - packet type identifier |
| 3 | 1 | Byte | 1 | Type; Unknown value between 1 and 3. |
| 4 | 1 | ServerToClient.GuildMemberRole |  | Role |
| 5 | 10 | String |  | PlayerName |