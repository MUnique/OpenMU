# C1 22 - PickupItemRequest075 (by client)

## Is sent when

A player requests to pick up an item which is laying on the ground in the near of the players character.

## Causes the following actions on the server side

If the player is allowed to pick the item up, and is the first player which tried that, it tries to add the item to the inventory. The server sends a response about the result of the request.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x22  | Packet header - packet type identifier |
| 3 | 2 | ShortBigEndian |  | ItemId |