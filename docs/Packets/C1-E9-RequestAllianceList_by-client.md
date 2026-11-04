# C1 E9 - RequestAllianceList (by client)

## Is sent when

The player opens the alliance list dialog.

## Causes the following actions on the server side

The server answers with the list of the guilds of the alliance.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   3   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xE9  | Packet header - packet type identifier |