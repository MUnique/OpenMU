# C1 F1 01 - LoginResponse (by server)

## Is sent when

After the login request has been processed by the server.

## Causes the following actions on the client side

Shows the result. When it was successful, the client proceeds by sending a character list request.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 1 | LoginResult |  | Success |

### LoginResult Enum

The result of a login request.

| Value | Name | Description |
|-------|------|-------------|
| 0 | InvalidPassword | The password was wrong. |
| 1 | Okay | The login succeeded. |
| 2 | AccountInvalid | The account is invalid. |
| 3 | AccountAlreadyConnected | The account is already connected. |
| 4 | ServerIsFull | The server is full. |
| 5 | AccountBlocked | The account is blocked. |
| 6 | WrongVersion | The game client has the wrong version. |
| 7 | ConnectionError | An internal error occured during connection. |
| 8 | ConnectionClosed3Fails | Connection closed because of three failed login requests. |
| 9 | NoChargeInfo | There is no payment information. |
| 10 | SubscriptionTermOver | The subscription term is over. |
| 11 | SubscriptionTimeOver | The subscription time is over. |
| 14 | TemporaryBlocked | The account is temporarily blocked. |
| 17 | OnlyPlayersOver15Yrs | Only players over 15 years are allowed to connect. |
| 210 | BadCountry | The client connected from a blocked country. |