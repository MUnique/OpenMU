# C3 B1 01 - ServerChangeAuthentication (by client)

## Is sent when

After the client connected to another server due map change.

## Causes the following actions on the server side

The player spawns on the new server.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   69   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 12 | Binary |  | AccountXor3 |
| 16 | 12 | Binary |  | CharacterNameXor3 |
| 28 | 4 | IntegerLittleEndian |  | AuthCode1 |
| 32 | 4 | IntegerLittleEndian |  | AuthCode2 |
| 36 | 4 | IntegerLittleEndian |  | AuthCode3 |
| 40 | 4 | IntegerLittleEndian |  | AuthCode4 |
| 44 | 4 | IntegerLittleEndian |  | TickCount |
| 48 | 5 | Binary |  | ClientVersion |
| 53 | 16 | Binary |  | ClientSerial |