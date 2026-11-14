# C2 F5 09 - WarpListExtended (by server)

## Is sent when

After the character entered the game world, when the client is the open source client.

## Causes the following actions on the client side

The client shows these warps in its warp window (and accepts them for the warp chat command) instead of the warp list of its data files. Warps which the client knows keep the name of its data files; others get the name of this message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x09  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | Count |
| 6 | WarpListEntry.Length * Count | Array of WarpListEntry |  | Warps |

### WarpListEntry Structure

A warp which the player can select in the warp window.

Length: 44 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | Index; The index of the warp, which the client sends in the WarpCommandRequest. |
| 2 | 2 | ShortLittleEndian |  | MapNumber; The number of the map to which the warp leads. |
| 4 | 2 | ShortLittleEndian |  | LevelRequirement; The required character level, as configured. Character classes with a reduced warp level requirement get the reduction on top of it, like in the warp list of the data files. |
| 8 | 4 | IntegerLittleEndian |  | Costs; The zen which the warp costs. |
| 12 | 32 | String |  | Name; The name of the warp, in the language of the player. |