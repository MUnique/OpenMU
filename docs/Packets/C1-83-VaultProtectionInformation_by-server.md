# C1 83 - VaultProtectionInformation (by server)

## Is sent when

After the player requested to open the vault.

## Causes the following actions on the client side

The game client updates the UI to show the current vault protection state.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x83  | Packet header - packet type identifier |
| 3 | 1 | VaultProtectionState |  | ProtectionState |

### VaultProtectionState Enum

Defines the vault protection state.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Unprotected | The vault is unprotected. |
| 1 | Locked | The vault is protected and locked. To move items or money, the player needs to unlock it. |
| 10 | UnlockFailedByWrongPin | The vault is protected and locked. The user-requested unlock failed by a wrong pin. |
| 11 | SetPinFailedBecauseLock | The vault is protected and locked and the player-requested pin setting failed because of the lock. |
| 12 | Unlocked | The vault is protected, but was unlocked by the player. |
| 13 | RemovePinFailedByWrongPassword | The vault is protected and the player-requested pin removal failed by using the wrong password. |