# C1 83 01 - SetVaultPin (by client)

## Is sent when

The player wants to set a new pin for the vault when it's in unlocked state.

## Causes the following actions on the server side

The vault pin is set. VaultProtectionInformation is sent as response.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   27   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x83  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | Pin |
| 6 | 20 | String |  | Password; The password of the account, which is required to set a new vault pin. |