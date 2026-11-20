# C3 F1 10 - LoginWithTicket (by client)

## Is sent when

The player tries to log into the game with a login ticket instead of a password, e.g. after signing in on a website.

## Causes the following actions on the server side

The server redeems the ticket. A ticket can be redeemed once, and only until it expires. If it's valid, the login proceeds like with a password and the server answers with a LoginResponse; if not, the result is AccountInvalid.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   72   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x10  | Packet header - sub packet type identifier |
| 4 | 43 | String |  | Token; The token of the ticket: 256 random bits, encoded as base64url without padding. |
| 47 | 4 | IntegerBigEndian |  | TickCount |
| 51 | 5 | Binary |  | ClientVersion |
| 56 | 16 | Binary |  | ClientSerial |