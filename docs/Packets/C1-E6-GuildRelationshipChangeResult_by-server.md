# C1 E6 - GuildRelationshipChangeResult (by server)

## Is sent when

The result of a guild relationship change request (alliance or hostility) is sent back to the requester.

## Causes the following actions on the client side

The requester sees the result of the relationship change.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE6  | Packet header - packet type identifier |
| 3 | 1 | GuildRelationshipType |  | RelationshipType |
| 4 | 1 | GuildRelationshipRequestType |  | RequestType |
| 5 | 1 | GuildRelationshipChangeResultType |  | Result |
| 6 | 2 | ShortBigEndian |  | GuildMasterId |

### GuildRelationshipChangeResultType Enum

Defines the result of a guild relationship change request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Failed | The request failed. |
| 1 | Success | The request was successful. |
| 16 | GuildNotFound | GUILD_ANS_NOTEXIST_GUILD: The guild does not exist. |
| 16 | FailedDuringCastleSiege | GUILD_ANS_UNIONFAIL_BY_CASTLE: Alliance function will be restricted due to the Castle Siege. |
| 17 | NoAuthorization | GUILD_ANS_NOTEXIST_PERMISSION: No authorization to perform this action. |
| 18 | NotExistExtraStatus | GUILD_ANS_NOTEXIST_EXTRA_STATUS: The extra status does not exist. |
| 19 | NotExistExtraType | GUILD_ANS_NOTEXIST_EXTRA_TYPE: The extra type does not exist. |
| 21 | AlreadyInAlliance | GUILD_ANS_EXIST_RELATIONSHIP_UNION: The guild already has an alliance relationship. |
| 22 | AlreadyInHostility | GUILD_ANS_EXIST_RELATIONSHIP_RIVAL: The guild already has a hostility relationship. |
| 23 | GuildAllianceExists | GUILD_ANS_EXIST_UNION: A guild alliance already exists. |
| 24 | HostileGuildExists | GUILD_ANS_EXIST_RIVAL: A hostile guild already exists. |
| 25 | GuildAllianceDoesNotExist | GUILD_ANS_NOTEXIST_UNION: The guild alliance does not exist. |
| 26 | HostileGuildDoesNotExist | GUILD_ANS_NOTEXIST_RIVAL: The hostile guild does not exist. |
| 27 | NotMasterOfGuildAlliance | GUILD_ANS_NOT_UNION_MASTER: The player is not the master of the guild alliance. |
| 28 | NotGuildRival | GUILD_ANS_NOT_GUILD_RIVAL: The guild is not a rival guild. |
| 29 | IncompleteRequirementsToCreateAlliance | GUILD_ANS_CANNOT_BE_UNION_MASTER_GUILD: The requirements to create an alliance are incomplete. |
| 30 | MaximumNumberOfGuildsInAllianceReached | GUILD_ANS_EXCEED_MAX_UNION_MEMBER: The maximum number of guilds in the alliance has been reached. |
| 32 | RequestCancelled | GUILD_ANS_CANCEL_REQUEST: The request has been cancelled. |
| 161 | AllianceMasterNotInGens | GUILD_ANS_UNION_MASTER_NOT_GENS: The alliance master is not in a Gens. |
| 162 | GuildMasterNotInGens | GUILD_ANS_GUILD_MASTER_NOT_GENS: The guild master is not in a Gens. |
| 163 | DifferentGens | GUILD_ANS_UNION_MASTER_DISAGREE_GENS: The alliance master and guild master belong to different Gens. |

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