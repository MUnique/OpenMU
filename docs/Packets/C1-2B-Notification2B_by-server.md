# C1 2B - Notification2B (by server)

## Is sent when

Unidentified notification; the client only evaluates one flag.

## Causes the following actions on the client side

The client shows one of two messages, depending on the flag.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x2B  | Packet header - packet type identifier |
| 3 | 1 | Boolean |  | Flag |