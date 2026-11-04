# C1 B7 04 - WeaponExplosionRequest (by client)

## Is sent when

After the player fired a catapult and hit another catapult.

## Causes the following actions on the server side

The server damages the other catapult.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xB7  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x04  | Packet header - sub packet type identifier |
| 4 | 2 | ShortBigEndian |  | CatapultId |