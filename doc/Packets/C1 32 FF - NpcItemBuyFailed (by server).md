# C1 32 FF - NpcItemBuyFailed (by server)

## Is sent when

The request of buying an item from a NPC failed.

## Causes the following actions on the client side

The client is responsive again. Without this message, it may stuck.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x32  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0xFF  | Packet header - sub packet type identifier |