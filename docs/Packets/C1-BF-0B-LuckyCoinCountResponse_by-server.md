# C1 BF 0B - LuckyCoinCountResponse (by server)

## Is sent when

After the player opened the lucky coin registration dialog of the NPC Delgado, which requests the count of the registered coins.

## Causes the following actions on the client side

The client shows the count of the registered lucky coins in the dialog.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0B  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | Count |