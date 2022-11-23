# C1 F3 30 - SaveKeyConfiguration (by client)

## Is sent when

When leaving the game world with a character.

## Causes the following actions on the server side

The server saves this configuration in its database.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x30  | Packet header - sub packet type identifier |
| 4 |  | Binary |  | Configuration; The binary data of the key configuration |