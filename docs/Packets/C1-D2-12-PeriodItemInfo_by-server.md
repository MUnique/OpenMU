# C1 D2 12 - PeriodItemInfo (by server)

## Is sent when

For each period (time limited) item of the character.

## Causes the following actions on the client side

The client stores the expiration date for the item at the given slot (inventory slots below 12 are equipped items).

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x12  | Packet header - sub packet type identifier |
| 6 | 2 | ShortLittleEndian |  | ItemSlot |
| 8 | 4 | IntegerLittleEndian |  | ExpirationDate |