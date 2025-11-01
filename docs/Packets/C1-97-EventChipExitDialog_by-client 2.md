# C1 97 - EventChipExitDialog (by client)

## Is sent when

The player requests to close the event chip dialog.

## Causes the following actions on the server side

The event chip dialog will be closed.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x97  | Packet header - packet type identifier |