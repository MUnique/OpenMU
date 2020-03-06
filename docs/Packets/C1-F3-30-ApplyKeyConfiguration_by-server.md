# C1 F3 30 - ApplyKeyConfiguration (by server)

## Is sent when

When entering the game world with a character.

## Causes the following actions on the client side

The client restores this configuration in its user interface.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x30  | Packet header - sub packet type identifier |
| 4 |  | Binary |  | Configuration; The binary data of the key configuration |