# C1 47 - PartyMemberItemPickedUp (by server)

## Is sent when

After a party member picked up an item.

## Causes the following actions on the client side

The client shows a message with the item name.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   16   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x47  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | PlayerId |
| 8 | 4 | IntegerLittleEndian |  | ItemInfo; Bits 0-12: item number (group * 512 + number); bit 13: excellent; bit 14: ancient; bit 15: 380 option; bit 16/17: further option flags evaluated by the client. |
| 12 | 1 | Byte |  | ItemLevel |