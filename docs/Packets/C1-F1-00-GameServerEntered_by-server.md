# C1 F1 00 - GameServerEntered (by server)

## Is sent when

After a game client has connected to the game.

## Causes the following actions on the client side

It shows the login dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF1  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x00  | Packet header - sub packet type identifier |
| 4 | 1 | Boolean | true | Success |
| 5 | 2 | ShortBigEndian |  | PlayerId |
| 7 | 5 | String |  | VersionString |
| 7 | 5 | Binary |  | Version |