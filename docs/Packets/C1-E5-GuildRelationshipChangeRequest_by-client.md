# C1 E5 - GuildRelationshipChangeRequest (by client)

## Is sent when

A guild master sends a request to another guild master about changing the relationship between their guilds.

## Causes the following actions on the server side

The server sends a response with the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE5  | Packet header - packet type identifier |
| 3 | 1 | GuildRelationshipType |  | RelationshipType |
| 4 | 1 | GuildRequestType |  | RequestType |
| 5 | 2 | ShortLittleEndian |  | TargetPlayerId |

### GuildRelationshipType Enum

Describes the relationship type between guilds.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined relationship type. |
| 1 | Alliance | The alliance relationship type. |
| 2 | Hostility | The hostility relationship type. |

### GuildRequestType Enum

Describes the request type.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined request type. |
| 1 | Join | The join type. |
| 2 | Leave | The leave type. |