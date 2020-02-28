# C1 02 - PatchCheckRequest (by client)

## Is sent when

This packet is sent by the client (launcher) to check if the patch version is high enough to be able to connect to the server.

## Causes the following actions on the server side

The connect server will check the version and sends a 'PatchVersionOkay' or a 'ClientNeedsPatch' message.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x02  | Packet header - packet type identifier |
| 3 | 1 | Byte |  | MajorVersion |
| 4 | 1 | Byte |  | MinorVersion |
| 5 | 1 | Byte |  | PatchVersion |