# C2 68 - ViewportPreviewObjects (by server)

## Is sent when

The server sends preview objects (e.g. players or monsters of an event map) to be shown.

## Causes the following actions on the client side

The client adds the objects to the scene.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0x68  | Packet header - packet type identifier |
| 4 | 1 | Byte |  | ObjectCount |
| 5 |  | Binary |  | Objects; Entries of variable size, 25 + BuffCount bytes each: Kind (1 = player, 2/3 = monster/NPC) @0, class (player) or monster number (BE, monster/NPC) @1-2, object id (BE, highest bit masked) @3-4, X @5, Y @6, appearance (17 bytes, player) @7, BuffCount @24, buff ids @25. |