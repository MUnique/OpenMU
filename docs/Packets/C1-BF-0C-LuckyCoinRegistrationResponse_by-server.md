# C1 BF 0C - LuckyCoinRegistrationResponse (by server)

## Is sent when

After the player requested to register a lucky coin at the NPC Delgado.

## Causes the following actions on the client side

The client unlocks the register button. When the coin was registered, it shows the new count of the registered coins; otherwise, it shows a message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0C  | Packet header - sub packet type identifier |
| 4 | 1 | LuckyCoinRegistrationResult |  | Result |
| 8 | 4 | IntegerLittleEndian |  | Count |

### LuckyCoinRegistrationResult Enum

Defines the result of the lucky coin registration request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | NotEnoughItems | The player has no lucky coin in the inventory. |
| 1 | Registered | The lucky coin was registered. |
| 100 | AlreadyApplied | The account already applied, and it can only apply once. |