# C1 F3 06 - CharacterStatIncreaseResponseExtended (by server)

## Is sent when

After the server processed a character stat increase request packet.

## Causes the following actions on the client side

If it was successful, adds a point to the requested stat type.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   24   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x06  | Packet header - sub packet type identifier |
| 4 | 1 | CharacterStatAttribute |  | Attribute |
| 6 | 2 | ShortLittleEndian |  | AddedAmount |
| 8 | 4 | IntegerLittleEndian |  | UpdatedMaximumHealth |
| 12 | 4 | IntegerLittleEndian |  | UpdatedMaximumMana |
| 16 | 4 | IntegerLittleEndian |  | UpdatedMaximumShield |
| 20 | 4 | IntegerLittleEndian |  | UpdatedMaximumAbility |

### CharacterStatAttribute Enum

Defines the type of a character stat attribute.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Strength | The strength attribute. |
| 1 | Agility | The agility attribute. |
| 2 | Vitality | The vitality attribute. |
| 3 | Energy | The energy attribute. |
| 4 | Leadership | The leadership attribute. |