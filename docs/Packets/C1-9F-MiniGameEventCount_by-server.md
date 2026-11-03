# C1 9F - MiniGameEventCount (by server)

## Is sent when

After the player requested the remaining entry count of a mini game event.

## Causes the following actions on the client side

The client shows the remaining entry count.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x9F  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | EventType |
| 4 | 1 | Byte |  | RemainingEnterCount |