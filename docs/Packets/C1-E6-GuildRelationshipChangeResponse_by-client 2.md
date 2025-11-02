# C1 E6 - GuildRelationshipChangeResponse (by client)

## Is sent when

A guild master answered the request to another guild master about changing the relationship between their guilds.

## Causes the following actions on the server side

The server sends a response back to the requester. If the guild master agreed, it takes the necessary actions.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE6  | Packet header - packet type identifier |
| 3 | 1 | GuildRelationshipType |  | RelationshipType |
| 4 | 1 | GuildRequestType |  | RequestType |
| 5 | 1 | Boolean |  | Response |
| 6 | 2 | ShortLittleEndian |  | TargetPlayerId |

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