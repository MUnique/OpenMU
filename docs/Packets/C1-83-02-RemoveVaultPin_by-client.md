# C1 83 02 - RemoveVaultPin (by client)

## Is sent when

The player wants to remove the pin for the vault when it's in unlocked state.

## Causes the following actions on the server side

The vault pin is removed. VaultProtectionInformation is sent as response.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   27   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x83  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 6 | 20 | String |  | Password; The password of the account, which is required to remove the vault pin. |