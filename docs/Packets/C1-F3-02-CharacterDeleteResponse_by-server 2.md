# C1 F3 02 - CharacterDeleteResponse (by server)

## Is sent when

After the server processed a character delete response of the client.

## Causes the following actions on the client side

If successful, the character is deleted from the character selection screen. Otherwise, a message is shown.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 1 | CharacterDeleteResult |  | Result |

### CharacterDeleteResult Enum

Result of a character delete request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | Unsuccessful | Deleting was not successful |
| 1 | Successful | Deleting was successful |
| 2 | WrongSecurityCode | Deleting was not successful because a wrong security code was entered |