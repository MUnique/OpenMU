# C1 B2 16 - CastleSiegeCrownStateUpdate (by server)

## Is sent when

The server updates the state of the castle crown during the siege.

## Causes the following actions on the client side

The client updates the crown state display.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x16  | Packet header - sub packet type identifier |
| 4 | 1 | CastleSiegeCrownState |  | State |

### CastleSiegeCrownState Enum

Defines the availability of the castle crown.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Accessible | The crown is accessible. |
| 1 | Protected | The crown is protected. |
| 2 | RegistrationSucceeded | A guild successfully registered the crown. |