# C1 88 - ItemCraftingPreviewExtended (by server)

## Is sent when

After the items of the crafting dialog changed, e.g. at the chaos machine.

## Causes the following actions on the client side

The game client shows the success rate and the price of the crafting which matches the items, as they are calculated by the server, instead of calculating them by itself.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x88  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | MixType; The number of the crafting which matches the items, which is also sent by the client to execute it. 0, if the items don't match any crafting. |
| 4 | 1 | Byte |  | SuccessRate; The success rate in percent, including the crafting bonus of the character. |
| 8 | 4 | IntegerLittleEndian |  | Price; The price in zen, without the tax of the chaos machine. |