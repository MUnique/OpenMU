# C1 31 - CloseNpcRequest (by client)

## Is sent when

A player closes the dialog which was opened by an interaction with a NPC.

## Causes the following actions on the server side

The server updates the state of the player accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x31  | Packet header - packet type identifier |