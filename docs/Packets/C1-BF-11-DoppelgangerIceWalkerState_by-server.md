# C1 BF 11 - DoppelgangerIceWalkerState (by server)

## Is sent when

The ice walker appeared on or disappeared from the path during the doppelganger event.

## Causes the following actions on the client side

The client shows or hides the ice walker icon on the progress bar of the doppelganger frame.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x11  | Packet header - sub packet type identifier |
| 4 | 1 | IceWalkerState |  | State |
| 5 | 1 | Byte |  | Position; The position index of the ice walker on the path, between 0 (start) and 22 (magic circle). |

### IceWalkerState Enum

The state of the ice walker.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Appeared | The ice walker is present at the given position. |
| 1 | Disappeared | The ice walker was killed or disappeared. |