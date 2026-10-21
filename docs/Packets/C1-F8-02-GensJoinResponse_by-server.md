# C1 F8 02 - GensJoinResponse (by server)

## Is sent when

After the player requested to join a gens at one of the gens NPCs.

## Causes the following actions on the client side

The npc dialog shows the result. When the player joined, the client assigns the gens to the own character.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF8  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | GensJoinResult |  | Result |
| 5 | 1 | GensType |  | GensType |

### GensJoinResult Enum

Defines the result of the gens join request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The player joined the gens. |
| 1 | AlreadyJoined | The player is already member of a gens. |
| 2 | LeftRecently | The player left a gens recently and has to wait before joining again. |
| 3 | LevelTooLow | The level of the character is too low. |
| 4 | GuildInDifferentGens | The guild of the player is part of a different gens. The client shows this for members of a guild. |
| 5 | GuildMasterNotInGens | The guild master is not member of the gens. |
| 6 | InParty | The player is in a party. |
| 7 | GuildAllianceMember | The guild of the player is part of a guild alliance. |

### GensType Enum

Describes the gens type.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined gens type, e.g. when the player is not a member of a gens. |
| 1 | Duprian | The Duprian gens. |
| 2 | Vanert | The Vanert gens. |