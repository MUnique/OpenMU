# C1 87 - CloseChaosMachineRequest (by client)

## Is sent when

A player closes the dialog which was opened by an interaction with the chaos machine goblin.

## Causes the following actions on the client side

The server updates the state of the player accordingly.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x87  | Packet header - packet type identifier |