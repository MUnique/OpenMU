# C1 95 - EventChipRegistrationRequest (by client)

## Is sent when

The player registers an event item at an NPC, usually the golden archer.

## Causes the following actions on the server side

A response is sent back to the client with the current event chip count.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x95  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Type |
| 4 | 1 | Byte |  | ItemIndex |