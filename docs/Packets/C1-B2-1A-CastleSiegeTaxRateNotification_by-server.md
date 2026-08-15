# C1 B2 1A - CastleSiegeTaxRateNotification (by server)

## Is sent when

The castle owner changes a server-wide chaos machine or store tax rate.

## Causes the following actions on the client side

The client updates the applicable tax rate.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x1A  | Packet header - sub packet type identifier |
| 4 | 1 | CastleSiegeTaxType |  | TaxType |
| 5 | 1 | Byte |  | TaxRate |

### CastleSiegeTaxType Enum

Defines the castle tax or fee being changed.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Undefined | No tax type is selected. |
| 1 | ChaosMachine | The Chaos Machine tax rate. |
| 2 | Store | The NPC store tax rate. |
| 3 | HuntingZoneEntranceFee | The hunting-zone entrance fee. |