# C1 9E - PlaySoundEffect (by server)

## Is sent when

The server wants the client to play a sound effect.

## Causes the following actions on the client side

The client plays the sound effect.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   6   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0x9E  | Packet header - packet type identifier |
| 4 | 2 | ShortLittleEndian |  | EffectNumber |