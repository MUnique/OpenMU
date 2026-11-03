# C1 D0 00 - RequestD000 (by client)

## Is sent when

Sent by the 1.04d client; its purpose is not identified yet.

## Causes the following actions on the server side

Unknown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xD0  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |