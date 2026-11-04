# C1 B2 10 - CastleSiegeTaxMoneyWithdraw (by client)

## Is sent when

The guild master wants to withdraw the tax money from the castle npc.

## Causes the following actions on the server side

The server moves the money into the inventory of the guild master.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x10  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerBigEndian |  | Amount |