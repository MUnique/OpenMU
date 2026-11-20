# C2 F5 0B - WarpRequirementsExtended (by server)

## Is sent when

Right after the WarpListExtended, when the client is the open source client and at least one warp of the list requires a minimum master level or a minimum number of resets. It contains only the warps which have such a requirement.

## Causes the following actions on the client side

The client remembers the minimums of these warps and shows a warp in its warp window as unavailable (greyed out) while the character doesn't reach them. Warps which aren't in this message have no such requirement. The server checks the requirements again when the character asks for the warp.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x0B  | Packet header - sub packet type identifier |
| 5 | 1 | Byte |  | Count |
| 6 | WarpRequirementEntry.Length * Count | Array of WarpRequirementEntry |  | Requirements |

### WarpRequirementEntry Structure

The minimum master level and number of resets which a warp requires.

Length: 8 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | Index; The index of the warp, as in the WarpListExtended. |
| 2 | 2 | ShortLittleEndian |  | MasterLevel; The minimum master level; 0 when the warp doesn't require one. |
| 4 | 4 | IntegerLittleEndian |  | Resets; The minimum number of resets; 0 when the warp doesn't require one. |