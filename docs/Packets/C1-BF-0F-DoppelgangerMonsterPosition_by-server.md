# C1 BF 0F - DoppelgangerMonsterPosition (by server)

## Is sent when

The position of the most advanced monster on the path to the magic circle changed during the doppelganger event.

## Causes the following actions on the client side

The client updates the monster progress bar of the doppelganger frame.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   5   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBF  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0F  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | Position; The position index on the path, between 0 (start) and 22 (magic circle). |