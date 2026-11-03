# C1 34 - ItemRepairResponse (by server)

## Is sent when

After the player repaired an item.

## Causes the following actions on the client side

If Money is not 0, the client sets the inventory money to this value and plays the repair sound.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x34  | Packet header - packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Money; The money of the player after the repair. 0 means the repair failed. |