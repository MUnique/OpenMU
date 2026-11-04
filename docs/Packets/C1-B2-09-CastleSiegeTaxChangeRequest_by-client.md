# C1 B2 09 - CastleSiegeTaxChangeRequest (by client)

## Is sent when

The guild master wants to change the tax rate in the castle npc.

## Causes the following actions on the server side

The server changes the tax rates accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   9   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |
| 4 | 1 | CastleSiegeTaxType |  | TaxType |
| 5 | 4 | IntegerBigEndian |  | TaxValue; The percentage rate for shop and Chaos Machine taxes, or the entrance fee amount for the hunting zone. |

### CastleSiegeTaxType Enum

Defines the castle tax or fee being changed.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | No tax type is selected. |
| 1 | ChaosMachine | The Chaos Machine tax rate. |
| 2 | Store | The NPC store tax rate. |
| 3 | HuntingZoneEntranceFee | The hunting-zone entrance fee. |