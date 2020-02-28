# C1 F3 02 - DeleteCharacter (by client)

## Is sent when

The game client is at the character selection screen and the player requests to delete an existing character.

## Causes the following actions on the server side

The server checks if the player transmitted the correct security code and if the character actually exists. If all is valid, it deletes the character from the account. It then sends a response with a result code back to the game client.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF3  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x02  | Packet header - sub packet type identifier |
| 4 | 10 | String |  | Name; The name of the character which should be deleted. |
| 14 |  | String |  | SecurityCode; A security code (7 bytes long). Some game clients/servers also expect to transmit the account password (up to 20 bytes long) here. In OpenMU, we work with the security here, but are not limiting to a length of 7 bytes. |