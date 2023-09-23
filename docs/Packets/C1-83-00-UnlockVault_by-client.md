# C1 83 00 - UnlockVault (by client)

## Is sent when

The player wants to unlock the protected vault with a pin.

## Causes the following actions on the server side

The vault lock state on the server is updated. VaultProtectionInformation is sent as response.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   7   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x83  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | Pin |