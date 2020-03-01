# C1 F3 06 - IncreaseCharacterStatPoint (by client)

## Is sent when

The player decides to add a stat point to a specific stat type, by pressing a plus-button in the character info menu.

## Causes the following actions on the server side

The server checks if a level-up-point is available. If yes, it adds the point to the specified stat type. It sends a response back to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 1 | CharacterStatAttribute |  | StatType |

### CharacterStatAttribute Enum

Defines the type of a character stat attribute.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Strength | The strength attribute. |
| 1 | Agility | The agility attribute. |
| 2 | Vitality | The vitality attribute. |
| 3 | Energy | The energy attribute. |
| 4 | Leadership | The leadership attribute. |