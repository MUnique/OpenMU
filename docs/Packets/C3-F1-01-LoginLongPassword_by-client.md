# C3 F1 01 - LoginLongPassword (by client)

## Is sent when

The player tries to log into the game.

## Causes the following actions on the server side

The server is authenticating the sent login name and password. If it's correct, the state of the player is proceeding to be logged in.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |      | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 10 | Binary |  | Username; The user name, "encrypted" with Xor3. |
| 14 | 20 | Binary |  | Password; The password, "encrypted" with Xor3. |
| 34 | 4 | IntegerBigEndian |  | TickCount |
| 38 | 5 | Binary |  | ClientVersion |