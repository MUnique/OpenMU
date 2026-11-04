# C1 BD 03 - CrywolfContractRequest (by client)

## Is sent when

A player wants to make a contract at the crywolf statue for the crywolf event.

## Causes the following actions on the server side

The server tries to enter a contract with the player and the specified statue.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x03  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | StatueId |