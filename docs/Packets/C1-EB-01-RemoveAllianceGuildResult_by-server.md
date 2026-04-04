# C1 EB 01 - RemoveAllianceGuildResult (by server)

## Is sent when

A guild master sent a message to kick the guild from the alliance and it has been processed.

## Causes the following actions on the client side

The list of guilds is updated accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xEB  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean |  | Result |
| 5 | 1 | GuildRelationshipRequestType |  | RequestType |
| 6 | 1 | GuildRelationshipType |  | RelationshipType |

### GuildRelationshipRequestType Enum

Describes the request type.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined request type. |
| 1 | Join | The join type. |
| 2 | Leave | The leave type. |

### GuildRelationshipType Enum

Describes the relationship type between guilds.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined relationship type. |
| 1 | Alliance | The alliance relationship type. |
| 2 | Hostility | The hostility relationship type. |