# C1 96 - MutoNumberRequest (by client)

## Is sent when

The player requests information about the Muto number. Unused.

## Causes the following actions on the server side

A response is sent back to the client with the current Muto number.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x96  | Packet header - packet type identifier |