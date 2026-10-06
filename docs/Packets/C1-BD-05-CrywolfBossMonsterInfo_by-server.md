# C1 BD 05 - CrywolfBossMonsterInfo (by server)

## Is sent when

Every five seconds during the battle.

## Causes the following actions on the client side

The client shows the health of Balgass and the number of Dark Elves.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xBD  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x05  | Packet header - sub packet type identifier |
| 4 | 4 | IntegerLittleEndian |  | BalgassHealthPercent; The health of Balgass in percent, or -1, when Balgass is not alive. |
| 8 | 1 | Byte |  | DarkElfCount; The number of alive Dark Elves. The client shows it as count of 12. |