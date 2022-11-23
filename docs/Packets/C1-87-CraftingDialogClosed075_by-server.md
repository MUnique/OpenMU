# C1 87 - CraftingDialogClosed075 (by server)

## Is sent when

After the player requested to close the crafting dialog, this confirmation is sent back to the client.

## Causes the following actions on the client side

The game client closes the crafting dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x87  | Packet header - packet type identifier |