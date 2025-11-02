# C1 F8 01 - GensJoinRequest (by client)

## Is sent when

The player has opened one of the gens NPCs and requests to join it.

## Causes the following actions on the server side

The server checks if the player is not in a gens already and joins the player to the selected gens.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF8  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 3 | 1 | GensType |  | GensType |

### GensType Enum

Describes the gens type.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | The undefined gens type. |
| 1 | Duprian | The Duprian gens. |
| 2 | Vanert | The Vanert gens. |