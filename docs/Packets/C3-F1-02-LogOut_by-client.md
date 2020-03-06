# C3 F1 02 - LogOut (by client)

## Is sent when

When the client wants to leave the game in various ways.

## Causes the following actions on the server side

Depending on the LogOutType, the game server does several checks and sends a response back to the client. If the request was successful, the game client either closes the game, goes back to server or character selection.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
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