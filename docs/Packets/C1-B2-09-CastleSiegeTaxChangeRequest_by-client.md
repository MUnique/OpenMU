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
| 4 | 1 | Byte |  | TaxType; 0=Undefined, 1=ChaosMachine, 2 = Normal, 3 = EntranceFeeLandOfTrials |
| 5 | 4 | IntegerBigEndian |  | TaxRate |