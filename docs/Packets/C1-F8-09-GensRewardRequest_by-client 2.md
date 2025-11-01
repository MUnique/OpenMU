# C1 F8 09 - GensRewardRequest (by client)

## Is sent when

The game client requests to get a reward from the gens npc.

## Causes the following actions on the server side

The server checks if the player has enough points to get the reward, and sends a response.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF8  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |
| 3 | 1 | GensType |  | GensType |

### GensType Enum

Describes the gens type.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined gens type. |
| 1 | Duprian | The Duprian gens. |
| 2 | Vanert | The Vanert gens. |