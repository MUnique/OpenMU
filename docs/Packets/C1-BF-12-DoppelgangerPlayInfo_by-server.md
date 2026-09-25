# C1 BF 12 - DoppelgangerPlayInfo (by server)

## Is sent when

Periodically (every second) while the doppelganger event is running.

## Causes the following actions on the client side

The client updates the remaining time and the positions of the party members on the progress bar of the doppelganger frame. Players which are not included are no longer shown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x12  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | RemainingSeconds |
| 6 | 1 | Byte |  | PlayerCount |
| 8 | PlayerPosition.Length * PlayerCount | Array of PlayerPosition |  | PlayerPositions |

### PlayerPosition Structure

The position of a player on the path.

Length: 4 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 2 | ShortLittleEndian |  | PlayerId |
| 2 | 1 | Byte |  | MapNumber; The map number of the player. It is ignored by the client. |
| 3 | 1 | Byte |  | Position; The position index of the player on the path, between 0 (start) and 22 (magic circle). |