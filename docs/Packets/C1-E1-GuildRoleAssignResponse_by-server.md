# C1 E1 - GuildRoleAssignResponse (by server)

## Is sent when

After a guild master requested to assign a role (e.g. battle master) to a guild member.

## Causes the following actions on the client side

The client shows a message depending on type and result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE1  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Type |
| 4 | 1 | Byte |  | Result |