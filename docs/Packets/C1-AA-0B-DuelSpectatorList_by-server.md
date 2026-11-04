# C1 AA 0B - DuelSpectatorList (by server)

## Is sent when

When a spectator joins or leaves a duel.

## Causes the following actions on the client side

The client updates the list of spectators.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   105   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xAA  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0B  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Count |
| 5 | DuelSpectator.Length *  | Array of DuelSpectator |  | Spectators |

### DuelSpectator Structure

Structure for a duel room entry.

Length: 10 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 10 | String |  | Name |