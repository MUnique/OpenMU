# C3 0E 00 - Ping (by client)

## Is sent when

This packet is sent by the client every few seconds. It contains the current "TickCount" of the client operating system and the attack speed of the selected character.

## Causes the following actions on the server side

By the original server this is used to detect speed hacks.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x0E  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | TickCount |
| 8 | 2 | ShortLittleEndian |  | AttackSpeed |