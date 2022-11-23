# C3 F9 01 - OpenNpcDialog (by server)

## Is sent when

The server acknowledges the requested opening of an npc dialog.

## Causes the following actions on the client side

The client opens the dialog of the specified npc.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC3  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF9  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x01  | Packet header - sub packet type identifier |
| 4 | 2 | ShortLittleEndian |  | NpcNumber |
| 8 | 4 | IntegerLittleEndian |  | GensContributionPoints |