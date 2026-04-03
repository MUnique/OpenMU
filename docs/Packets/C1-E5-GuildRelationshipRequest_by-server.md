# C1 E5 - GuildRelationshipRequest (by server)

## Is sent when

A guild master sent a relationship change request (alliance or hostility) and the server forwards this request to the target guild master.

## Causes the following actions on the client side

The target guild master (receiver of this message) sees the incoming request dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE5  | Packet header - packet type identifier |
| 3 | 1 | GuildRelationshipType |  | RelationshipType |
| 4 | 1 | GuildRelationshipRequestType |  | RequestType |
| 5 | 2 | ShortBigEndian |  | SenderId |

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