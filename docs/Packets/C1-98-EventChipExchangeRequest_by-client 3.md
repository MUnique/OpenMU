# C1 98 - EventChipExchangeRequest (by client)

## Is sent when

The player requests to exchange the event chips to something else.

## Causes the following actions on the server side

A response is sent back to the client with the exchange result.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x98  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | Type |