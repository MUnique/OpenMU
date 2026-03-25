# C1 E6 - GuildRelationshipChangeResult (by server)

## Is sent when

The result of a guild relationship change request (alliance or hostility) is sent back to the requester.

## Causes the following actions on the client side

The requester sees the result of the relationship change.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE6  | Packet header - packet type identifier |
| 3 | 1 | GuildRelationshipType |  | RelationshipType |
| 4 | 1 | GuildRelationshipRequestType |  | RequestType |
| 5 | 1 | Boolean |  | Success |

### GuildRelationshipType Enum

Describes the relationship type between guilds.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined relationship type. |
| 1 | Alliance | The alliance relationship type. |
| 2 | Hostility | The hostility relationship type. |

### GuildRelationshipRequestType Enum

Describes the request type.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined request type. |
| 1 | Join | The join type. |
| 2 | Leave | The leave type. |