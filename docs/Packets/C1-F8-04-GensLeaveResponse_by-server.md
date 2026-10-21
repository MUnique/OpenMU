# C1 F8 04 - GensLeaveResponse (by server)

## Is sent when

After the player requested to leave the gens at one of the gens NPCs.

## Causes the following actions on the client side

The npc dialog shows the result. When the player left, the client removes the gens of the own character.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF8  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 1 | GensLeaveResult |  | Result |

### GensLeaveResult Enum

Defines the result of the gens leave request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The player left the gens. |
| 1 | NotJoined | The player is not member of a gens. |
| 2 | GuildMasterCannotLeave | The player is a guild master, which can't leave the gens. |
| 3 | DifferentGensNpc | The player is member of a different gens than the one of the npc. |