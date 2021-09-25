# C1 91 - RequestEventRemainingTime (by client)

## Is sent when

The player requests to get the remaining time of the currently entered event.

## Causes the following actions on the server side

The remaining time is sent back to the client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x91  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | EventType |
| 4 | 1 | Byte |  | EventLevel |