# C1 B2 18 - CastleSiegeBattleProcess (by server)

## Is sent when

A guild starts accessing the crown or successfully takes ownership of the castle.

## Causes the following actions on the client side

The client announces the crown access or castle ownership change.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   13   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB2  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x18  | Packet header - sub packet type identifier |
| 4 | 1 | CastleSiegeBattleProcessState |  | State |
| 5 | 8 | String |  | GuildName |

### CastleSiegeBattleProcessState Enum

Defines a public Castle Siege battle-process announcement.

| Value | Name | Description |
|-------|------|-------------|
| 0 | CrownRegistrationStarted | A guild started registering the crown. |
| 1 | CrownRegistrationSucceeded | A guild successfully registered the crown. |