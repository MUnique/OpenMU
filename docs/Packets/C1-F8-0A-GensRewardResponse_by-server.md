# C1 F8 0A - GensRewardResponse (by server)

## Is sent when

After the player requested the gens ranking reward at one of the gens NPCs.

## Causes the following actions on the client side

The npc dialog shows the result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF8  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0A  | Packet header - sub packet type identifier |
| 4 | 1 | GensRewardResult |  | Result |

### GensRewardResult Enum

Defines the result of the gens reward request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Success | The player got the reward. |
| 1 | OutsideRewardPeriod | The rewards are only given out in the reward period, e.g. the first week of a month. |
| 2 | NotEligible | The player is not eligible for a reward. |
| 3 | InventoryFull | The inventory of the player has not enough space for the reward. |
| 4 | AlreadyClaimed | The player already got the reward. |
| 5 | DifferentGensNpc | The player is member of a different gens than the one of the npc. |
| 6 | NotJoined | The player is not member of a gens. |