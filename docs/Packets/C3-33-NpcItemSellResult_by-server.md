# C3 33 - NpcItemSellResult (by server)

## Is sent when

The result of a previous item sell request.

## Causes the following actions on the client side

The amount of specified money is set at the players inventory.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x33  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Success |
| 4 | 4 | IntegerLittleEndian |  | Money |