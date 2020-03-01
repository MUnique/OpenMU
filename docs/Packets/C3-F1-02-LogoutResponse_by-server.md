# C3 F1 02 - LogoutResponse (by server)

## Is sent when

After the logout request has been processed by the server.

## Causes the following actions on the client side

Depending on the result, the game client closes the game or changes to another selection screen.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | LogOutType |  | Type |

### LogOutType Enum

Describes the way how the player wants to leave the current game.

| Value | Name | Description |
|-------|------|-------------|
| 0 | CloseGame | The player wants to close the game. |
| 1 | BackToCharacterSelection | The player wants to go back to the character selection screen. |
| 2 | BackToServerSelection | The player wants to go back to the server selection screen. |