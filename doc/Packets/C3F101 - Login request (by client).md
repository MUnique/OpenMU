# C3 F1 01 - Login Request

## Is sent when ##
The player wants to login at the game server using his login name and password.


## Causes the following actions on the server side ##
The server checks if the login name and password are valid. If yes, it loads the Account and changes the PlayerState to Authenticated.
It answers with a Login Response Packet (e.g. C3 F1 01 01) where the last byte says if it was successful.


## Structure ##

|  Length  | Data type | Value | Description |
|----------|---------|-------------|---------|
| 1 | byte | 0xC3   | [Packet type](PacketTypes.md) |
| 1 | byte | [Length] | Packet header - length of the packet |
| 1 | byte | 0xF1   | Packet header - packet type identifier |
| 1 | byte | 0x01   | Packet header - packet sub type identifier for "login" |
|10 | string |      | Login name ("encrypted" with Xor3) |
|20 | string |      | Password ("encrypted" with Xor3) |
| 4 | uint |         | TickCount of the client |
| 5 | string |  | Client Version, e.g. "10404" for 1.04d |
| 16 | string |  | Client serial number |

